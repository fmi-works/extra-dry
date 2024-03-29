﻿using ExtraDry.Server.DataWarehouse.Builder;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

namespace ExtraDry.Server.DataWarehouse;

public class WarehouseModelBuilder {

    public void LoadSchema<T>() where T : DbContext
    {
        LoadSchema(typeof(T));
    }

    public void LoadSchema(Type contextType)
    {
        var entityTypes = GetEntities(contextType);
        var assemblies = entityTypes.Select(e => e.Assembly).Distinct().ToList();

        // Load enums, they're never dependent on anything.
        foreach(var enumType in GetEnums(assemblies)) {
            var dimensionAttribute = enumType.GetCustomAttribute<DimensionTableAttribute>();
            if(dimensionAttribute != null) {
                LoadEnumDimension(enumType, dimensionAttribute);
            }
        }

        // Load dimensions, needed before facts are loaded.
        foreach(var entity in entityTypes) {
            var dimensionAttribute = entity.GetCustomAttribute<DimensionTableAttribute>();
            if(dimensionAttribute != null) {
                LoadClassDimension(entity);
            }
        }

        // After dimensions are loaded, load spokes between them
        foreach(var dimension in DimensionTables.Values) {
            dimension.LoadSpokesDeferred();
        }

        // Finally load facts and their foreign keys to dimensions.
        foreach(var entity in entityTypes) {
            var factAttribute = entity.GetCustomAttribute<FactTableAttribute>();
            if(factAttribute != null) {
                LoadClassFact(entity);
            }
        }
    }

    public FactTableBuilder<T> Fact<T>() where T : class
    {
        try {
            return FactTables[typeof(T)] as FactTableBuilder<T> ?? throw new KeyNotFoundException();
        }
        catch(KeyNotFoundException) {
            throw new DryException($"No Fact table of type '{typeof(T).Name}' was defined.");
        }
    }

    public DimensionTableBuilder Dimension(Type type)
    {
        try {
            return DimensionTables[type] as DimensionTableBuilder ?? throw new KeyNotFoundException();
        }
        catch(KeyNotFoundException) {
            throw new DryException($"No Dimension table of type '{type.Name}' was defined.");
        }
    }

    public DimensionTableBuilder<T> Dimension<T>() where T : class
    {
        try {
            return DimensionTables[typeof(T)] as DimensionTableBuilder<T> ?? throw new KeyNotFoundException();
        }
        catch(KeyNotFoundException) {
            throw new DryException($"No Dimension table of type '{typeof(T).Name}' was defined.");
        }
    }

    public DimensionTableBuilder<EnumDimension> EnumDimension<T>() where T : Enum
    {
        return EnumDimension(typeof(T));
    }

    public bool HasDimension(Type type) => DimensionTables.ContainsKey(type);

    public DimensionTableBuilder<EnumDimension> EnumDimension(Type type) 
    {
        try {
            return DimensionTables[type] as DimensionTableBuilder<EnumDimension> ?? throw new KeyNotFoundException();
        }
        catch(KeyNotFoundException) {
            throw new DryException($"No Dimension table of type '{type.Name}' was defined.");
        }
    }

    public WarehouseModel Build()
    {
        var model = new WarehouseModel();
        foreach(var dimensionBuilder in DimensionTables.Values) {
            model.Dimensions.Add(dimensionBuilder.Build());
        }
        foreach(var factBuilder in FactTables.Values) {
            model.Facts.Add(factBuilder.Build());
        }
        return model;
    }

    internal bool HasTableNamed(string name) =>
        FactTables.Values.Any(e => string.Compare(e.TableName, name, StringComparison.InvariantCultureIgnoreCase) == 0) ||
        DimensionTables.Values.Any(e => string.Compare(e.TableName, name, StringComparison.InvariantCultureIgnoreCase) == 0);

    private void LoadEnumDimension(Type enumType, DimensionTableAttribute dimension)
    {
        if(LoadViaConstructor(typeof(DimensionTableBuilder<>), typeof(Builder.EnumDimension)) is DimensionTableBuilder builder) {
            ConfigureEnumDimension(enumType, dimension, builder);
            DimensionTables.Add(enumType, builder);
        }
    }

    private static void ConfigureEnumDimension(Type enumType, DimensionTableAttribute dimension, DimensionTableBuilder builder)
    {
        var name = dimension.Name ?? DataConverter.CamelCaseToTitleCase(enumType.Name);
        builder.HasName(name);
        builder.HasKey().HasName($"{name} ID");

        var stats = new EnumStats(enumType);

        builder.Attribute(nameof(Builder.EnumDimension.Name)).HasLength(stats.DisplayNameMaxLength());

        if(stats.HasShortName()) {
            builder.Attribute(nameof(Builder.EnumDimension.ShortName)).HasLength(stats.ShortNameMaxLength());
        }
        else {
            builder.Attribute(nameof(Builder.EnumDimension.ShortName)).IsIncluded(false);
        }

        if(stats.HasGroupName()) {
            builder.Attribute(nameof(Builder.EnumDimension.GroupName)).HasLength(stats.GroupNameMaxLength());
        }
        else {
            builder.Attribute(nameof(Builder.EnumDimension.GroupName)).IsIncluded(false);
        }

        if(stats.HasDescription()) {
            builder.Attribute(nameof(Builder.EnumDimension.Description)).HasLength(stats.DescriptionMaxLength());
        }
        else {
            builder.Attribute(nameof(Builder.EnumDimension.Description)).IsIncluded(false);
        }

        if(!stats.HasOrder()) {
            builder.Attribute(nameof(Builder.EnumDimension.Order)).IsIncluded(false);
        }

        LoadEnumBaseData(builder, stats);
    }

    private static void LoadEnumBaseData(DimensionTableBuilder builder, EnumStats enumStats)
    {
        var keyBuilder = builder.HasKey();
        var nameBuilder = builder.Attribute(nameof(Builder.EnumDimension.Name));
        var shortNameBuilder = builder.Attribute(nameof(Builder.EnumDimension.ShortName));
        var descriptionBuilder = builder.Attribute(nameof(Builder.EnumDimension.Description));
        var groupNameBuilder = builder.Attribute(nameof(Builder.EnumDimension.GroupName));
        var orderBuilder = builder.Attribute(nameof(Builder.EnumDimension.Order));
        foreach(var enumField in enumStats.Fields) {
            var data = new Dictionary<ColumnBuilder, object> {
                { keyBuilder, enumField.Value },
                { nameBuilder, enumField.DisplayName }
            };
            if(enumStats.HasShortName()) {
                data.Add(shortNameBuilder, enumField.ShortName ?? string.Empty);
            }
            if(enumStats.HasDescription()) {
                data.Add(descriptionBuilder, enumField.Description ?? string.Empty);
            }
            if(enumStats.HasGroupName()) {
                data.Add(groupNameBuilder, enumField.GroupName ?? string.Empty);
            }
            if(orderBuilder.Included) {
                // Default per https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute.order?view=net-6.0
                data.Add(orderBuilder, enumField.Order ?? 10000);
            }
            builder.HasData(data);
        }
    }

    private void LoadClassDimension(Type entity)
    {
        if(LoadViaConstructor(typeof(DimensionTableBuilder<>), entity) is DimensionTableBuilder builder) {
            DimensionTables.Add(entity, builder);
        }
    }

    private void LoadClassFact(Type entity)
    {
        if(LoadViaConstructor(typeof(FactTableBuilder<>), entity) is FactTableBuilder builder) {
            FactTables.Add(entity, builder);
        }
    }

    private object LoadViaConstructor(Type generic, Type entity) 
    {
        // To activate, need type, args and indicator that the constructor is not public, tricky to get right overload...
        var factTableBuilderType = generic.MakeGenericType(entity);
        var args = new object[] { this, entity };
        var binding = BindingFlags.NonPublic | BindingFlags.Instance;
        var binder = (Binder?)null;
        var culture = CultureInfo.InvariantCulture;
        try {
            return Activator.CreateInstance(factTableBuilderType, binding, binder, args, culture) 
                ?? throw new DryException("Couldn't create instance of FactBuilderTable");
        }
        catch(TargetInvocationException ex) when (ex.InnerException is DryException dryEx) {
            throw dryEx;
        }
    }

    private static IEnumerable<Type> GetEntities(Type tableType)
    {
        var properties = tableType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach(var property in properties) {
            var type = property.PropertyType;
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DbSet<>)) {
                var entityType = type.GetGenericArguments()[0];
                yield return entityType;
            }
        }
    }

    private static IEnumerable<Type> GetEnums(List<Assembly> assemblies)
    {
        foreach(var assembly in assemblies) {
            foreach(var type in assembly.GetTypes()) {
                if(type.IsEnum) {
                    yield return type;
                }
            }
        }
    }

    private Dictionary<Type, FactTableBuilder> FactTables { get; } = new();

    private Dictionary<Type, DimensionTableBuilder> DimensionTables { get; } = new();

}
