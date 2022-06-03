﻿using ExtraDry.Server.DataWarehouse.Builder;
using ExtraDry.Server.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ExtraDry.Server.DataWarehouse;

public class DataFactory {

    public DataFactory(WarehouseModel model, DbContext source, WarehouseContext target, ILogger<DataFactory> logger, DataFactoryOptions? options = null)
    {
        Model = model;
        Oltp = source;
        Olap = target;
        Logger = logger;
        Options = options ?? new();
    }

    public async Task MigrateAsync()
    {
        Logger.LogInformation("Checking for necessary migrations.");
        await Olap.Database.MigrateAsync(); // EF Schema
        foreach(var table in Model.Dimensions.Union(Model.Facts)) {
            var schema = JsonSerializer.Serialize(table);
            var updateInfo = await Olap.TableSyncs.FirstOrDefaultAsync(e => e.Table == table.Name);
            if(updateInfo == null) {
                Logger.LogInformation("No table for [{tableName}], creating new.", table.Name);
                await Olap.Database.BeginTransactionAsync();
                updateInfo = new DataTableSync { Schema = schema, Table = table.Name, SyncTimestamp = DateTime.MinValue };
                Olap.TableSyncs.Add(updateInfo);
                await CreateTargetTable(table, updateInfo);
                await Olap.Database.CommitTransactionAsync();
            }
            else if(updateInfo.Schema != schema) {
                Logger.LogInformation("Table for [{tableName}] obsolete, dropping and creating new.", table.Name);
                await Olap.Database.BeginTransactionAsync();
                updateInfo.Schema = schema;
                updateInfo.SyncTimestamp = DateTime.MinValue;
                await DropTargetTable(table);
                await CreateTargetTable(table, updateInfo);
                await Olap.Database.CommitTransactionAsync();
            }
            else {
                Logger.LogInformation("No changes detected for [{tableName}].", table.Name);
            }
        }
    }

    public async Task ProcessBatchesAsync()
    {
        await OptionallyApplyMigrationsAsync();
        var tables = Model.Dimensions.Union(Model.Facts).Where(e => e.SourceProperty != null);
        foreach(var table in tables) {
            await ProcessTableBatch(table);
        }
    }

    private async Task ProcessTableBatch(Table table)
    {
        Logger.LogDebug("Processing batch for [{tableName}]", table.Name);
        if(table.SourceProperty == null) {
            Logger.LogDebug("Table [{tableName}] not dynamic, batch load aborted", table.Name);
            return; // can't process changes on enums without a source property.
        }

        var batchStats = await Olap.TableSyncs.FirstOrDefaultAsync(e => e.Table == table.Name)
            ?? throw new DryException("Unable to process batch, stats missing, run MigrateAsync() first.");
        Logger.LogDebug("Most recent record for [{tableName}] was modified on {timestamp}", table.Name, batchStats.SyncTimestamp);

        // Dynamically build up something comparable to the following:
        // var batchIncoming = await Oltp.Companies
        //      .Where(e => e.Version.DateModified > batchStats.SyncTimestamp)
        //      .OrderBy(e => e.Version.DateModified)
        //      .Take(Options.BatchSize)
        //      .ToListAsync();
        // Dynamic used to access extensions methods manually, then once batch received get out of dynamic hell.
        var dbSet = (dynamic?)table.SourceProperty.GetValue(Oltp)
            ?? throw new DryException("Source context for model must match the source DbContext for the factory.");
        var where = LinqBuilder.WhereVersionModifiedAfter(dbSet, batchStats.SyncTimestamp);
        var ordered = LinqBuilder.OrderBy(where, "Id");
        var taken = Queryable.Take(ordered, Options.BatchSize);
        var batch = new List<object>();
        batch.AddRange(await EntityFrameworkQueryableExtensions.ToListAsync(taken));

        if(batch.Any()) {
            Logger.LogInformation("Modified entities on [{tableName}], processing {batchCount} upserts.", table.Name, batch.Count);
            var target = Model.Dimensions.First(e => e.Name == table.Name);
            foreach(var item in batch) {
                var sql = Upsert(target, item);
                Logger.LogTrace("Executing Upsert SQL: {sql}", sql);
                await Olap.Database.ExecuteSqlRawAsync(sql);
            }
            batchStats.SyncTimestamp = batch.Max(e => GetVersionInfo(e)?.DateModified ?? DateTime.MinValue);
            await Olap.SaveChangesAsync();
            Logger.LogInformation("Processed {batchCount} upserts on [{tableName}], updating sync timestamp to {timestamp}.", batch.Count, table.Name, batchStats.SyncTimestamp);
        }
        else {
            Logger.LogDebug("No entities modified on [{tableName}], batch completed with no changes.", table.Name);
        }
    }

    private VersionInfo? GetVersionInfo(object item)
    {
        var type = item.GetType();
        if(!VersionInfoProperties.ContainsKey(type)) {
            var property = type.GetProperties().FirstOrDefault(e => e.PropertyType == typeof(VersionInfo))
                ?? throw new DryException("Object does not have a VersionInfo property.");
            VersionInfoProperties[type] = property;
        }
        return VersionInfoProperties[type].GetValue(item) as VersionInfo;
    }
    private Dictionary<Type, PropertyInfo> VersionInfoProperties { get; } = new();

    private string Upsert(Table table, object entity)
    {
        var values = new Dictionary<string, object>();
        var key = table.KeyColumn.PropertyInfo?.GetValue(entity, null) ?? throw new DryException("No key value defined");
        foreach(var column in table.ValueColumns) {
            var value = column.PropertyInfo?.GetValue(entity, null) ?? column.Default;
            if(column.ColumnType == ColumnType.Integer) {
                // Ensure that enums that are marked as integer aren't created as strings.
                values.Add(column.Name, (int)value);
            }
            else {
                values.Add(column.Name, value);
            }
        }
        return Sql.Upsert(table, (int)key, values);
    }


    private async Task CreateTargetTable(Table table, DataTableSync updateInfo)
    {
        Logger.LogInformation("Creating warehouse table [{tableName}]", table.Name);
        var sqlTable = Sql.CreateTable(table);
        Logger.LogTrace("Executing Create Table SQL: {sql}", sqlTable);
        await Olap.Database.ExecuteSqlRawAsync(sqlTable);
        var sqlData = Sql.InsertData(table);
        if(!string.IsNullOrWhiteSpace(sqlData)) {
            Logger.LogTrace("Executing Insert Data SQL: {sql}", sqlData);
            // Enums have static data
            await Olap.Database.ExecuteSqlRawAsync(sqlData);
            updateInfo.SyncTimestamp = DateTime.UtcNow;
        }
        await Olap.SaveChangesAsync();
    }

    private async Task DropTargetTable(Table table)
    {
        var sqlDrop = Sql.DropTable(table);
        Logger.LogTrace("Executing Drop Table SQL: {sql}", sqlDrop);
        await Olap.Database.ExecuteSqlRawAsync(sqlDrop);
        await Olap.SaveChangesAsync();
    }

    private async Task OptionallyApplyMigrationsAsync()
    {
        if(Options.AutoMigrations && !migrationsApplied) {
            await MigrateAsync();
            migrationsApplied = true;
        }
    }

    private bool migrationsApplied = false;

    private WarehouseModel Model { get; }

    private DbContext Oltp { get; }

    private WarehouseContext Olap { get; }

    private DataFactoryOptions Options { get; }

    private ILogger<DataFactory> Logger { get; }

    private ISqlGenerator Sql { get; } = new SqlServerSqlGenerator();

}
