﻿using System.Reflection;

namespace ExtraDry.Server.DataWarehouse.Builder;

public class KeyBuilder : ColumnBuilder {

    internal KeyBuilder(TableBuilder tableBuilder, Type entityType, PropertyInfo propertyInfo) 
        : base(tableBuilder, entityType, propertyInfo) 
    { 
        SetName($"{tableBuilder.TableName} ID");
        SetType(ColumnType.Key);
    }

    public KeyBuilder HasName(string name)
    {
        SetName(name);
        return this;
    }

    protected override bool IsValidColumnType(ColumnType type)
    {
        return type == ColumnType.Key;
    }

    internal override Column Build()
    {
        return new Column(ColumnType.Key, ColumnName) {
            Nullable = false,
            PropertyInfo = PropertyInfo,
        };
    }

}
