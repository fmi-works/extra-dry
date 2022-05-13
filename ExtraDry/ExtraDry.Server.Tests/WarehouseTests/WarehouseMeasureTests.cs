﻿using ExtraDry.Core.DataWarehouse;
using ExtraDry.Server.DataWarehouse;
using Microsoft.EntityFrameworkCore;

namespace ExtraDry.Server.Tests.WarehouseTests;

public class WarehouseMeasureTests {

    [Theory]
    [InlineData("Measure Container ID", ColumnType.Key)] // Key Column naming convention
    [InlineData("Integer", ColumnType.Integer)] // Simple int property, no decoration or special handling
    [InlineData("Short", ColumnType.Integer)] // Simple short property, no decoration or special handling
    [InlineData("Float", ColumnType.Double)] // Simple float property, no decoration or special handling
    [InlineData("Double", ColumnType.Double)] // Simple double property, no decoration or special handling
    [InlineData("Gross", ColumnType.Decimal)] // Simple decimal property, no decoration or special handling
    [InlineData("Annual Revenue", ColumnType.Decimal)] // Simple decimal property, with naming conversion
    [InlineData("Double Revenue", ColumnType.Decimal)] // Decimal property with getter only, with naming conversion
    [InlineData("Gifts", ColumnType.Decimal)] // NotMapped, but also Measure so include it.
    [InlineData("Big Bucks", ColumnType.Decimal)] // Measure attribute rename
    public void MeasureIncluded(string title, ColumnType columnType)
    {
        var builder = new WarehouseModelBuilder();

        builder.LoadSchema<MeasureContext>();
        var warehouse = builder.Build();

        var fact = warehouse.Facts.Single(e => e.EntityType == typeof(MeasureContainer));
        Assert.Contains(fact.Columns, e => e.Name == title && e.ColumnType == columnType);
    }

    [Theory]
    [InlineData("ID")] // Key property doesn't slip through as measure.
    [InlineData("Name")] // String column not a measure.
    [InlineData("Sales")] // EF NotMappedAttribue suppresses by default.
    public void MeasureNotIncluded(string title)
    {
        var builder = new WarehouseModelBuilder();

        builder.LoadSchema<MeasureContext>();
        var warehouse = builder.Build();

        var fact = warehouse.Facts.Single(e => e.EntityType == typeof(MeasureContainer));
        Assert.DoesNotContain(fact.Columns, e => e.Name == title);
    }

    [Fact]
    public void FluentRenameOfMeasure()
    {
        var builder = new WarehouseModelBuilder();
        builder.LoadSchema<MeasureContext>();

        builder.FactTable<MeasureContainer>().Measure(e => e.GrossSalesLessCOGS).HasName("Gross Margin");

        var warehouse = builder.Build();
        var fact = warehouse.Facts.Single(e => e.EntityType == typeof(MeasureContainer));
        Assert.Contains(fact.Columns, e => e.Name == "Gross Margin" && e.ColumnType == ColumnType.Decimal);
    }

}

public class MeasureContext : DbContext {

    public DbSet<MeasureContainer> Measures { get; set; } = null!;
}

[FactTable]
public class MeasureContainer {

    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [Measure]
    public string Title { get; set; } = string.Empty;

    public int Integer { get; set; }

    public short Short { get; set; }

    public float Float { get; set; }

    public double Double { get; set; }

    public decimal Gross { get; set; }

    public decimal AnnualRevenue { get; set; }

    [Measure("Big Bucks")]
    public decimal BadName { get; set; }

    public decimal DoubleRevenue {
        get => 2 * AnnualRevenue;
    }

    [NotMapped]
    public decimal Sales { get; set; }

    [NotMapped, Measure]
    public decimal Gifts { get; set; }

    public decimal GrossSalesLessCOGS { get; set; }
}
