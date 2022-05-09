﻿namespace ExtraDry.Core.DataWarehouse;

/// <summary>
/// A data warehouse attribute of a dimension table.
/// </summary>
/// <remarks>
/// Note that this is a bizarrely named class as 'Attribute' is overloaded between the data warehouse and the framework.
/// However, in practice users of this class don't notice this oddity, and no synonyms exist with the right meaning.
/// E.g. Synonym 'Field' has an issue as Fields are a first class concept in C#.
/// E.g. Synonym 'Column' has an issue as ColumnAttribute conflicts with DataAnnotations.Schema.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class AttributeAttribute : Attribute {

    /// <summary>
    /// Declares a property as a data warehouse attribute with the name inferred from the property name.
    /// </summary>
    public AttributeAttribute() { }

    /// <summary>
    /// Declares a property as a data warehouse attribute with the name explicity defined.
    /// </summary>
    public AttributeAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The name to use for the data warehouse attribute column.  
    /// If `null`, then the name is inferred from the Property's name.
    /// </summary>
    public string? Name { get; set; }
}
