﻿// See https://aka.ms/new-console-template for more information
using ExtraDry.Server.DataWarehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Data;
using Sample.Shared;
using System.Text.Json;

var options = new JsonSerializerOptions() { WriteIndented = true };
//var json = JsonSerializer.Serialize(warehouse, options);
//Console.WriteLine(json);
//Console.WriteLine(warehouse.GenerateSql());

var builder = new WarehouseModelBuilder();
builder.LoadSchema<SampleContext>();

builder.Fact<Company>().Measure(e => e.AnnualRevenue).HasName("Big Bucks");

var model = builder.Build();
var compareJson = JsonSerializer.Serialize(model, options);


var connectionString = @"Server=(localdb)\mssqllocaldb;Database=ExtraDrySample;Trusted_Connection=True;";
var dbOptionsBuilder = new DbContextOptionsBuilder<SampleContext>().UseSqlServer(connectionString);
var databaseContext = new SampleContext(dbOptionsBuilder.Options);


var warehouseConnectionString = @"Server=(localdb)\mssqllocaldb;Database=ExtraDryWarehouse;Trusted_Connection=True;";
var warehouseOptionsBuilder = new DbContextOptionsBuilder<WarehouseContext>().UseSqlServer(warehouseConnectionString);
var warehouseContext = new WarehouseContext(warehouseOptionsBuilder.Options);


var services = new ServiceCollection();
services.AddLogging(configure => {
        configure.AddConsole();
        configure.SetMinimumLevel(LogLevel.Trace);
    });
var provider = services.BuildServiceProvider();
var dataFactoryLogger = provider.GetRequiredService<ILogger<DataFactory>>();


var factoryOptions = new DataFactoryOptions() {
    BatchSize = 10,
    AutoMigrations = true,
};


var factory = new DataFactory(model, databaseContext, warehouseContext, dataFactoryLogger, factoryOptions);
while(await factory.ProcessBatchesAsync() > 0) ;

