﻿namespace ExtraDry.Core;

/// <summary>
/// The base data type for an expansion field.  Closely follows the JSON data types, but adds
/// DateTime as a primitive type that is not present in JSON.
/// </summary>
public enum ExpandoDataType {

    /// <summary>
    /// Represents an instant in time, expressed as a date and time of day.
    /// </summary>
    /// <remarks>
    /// Extracted from JSON iff Text and exactly matches "yyyy-mm-ddThh:mm:ss[.fffffff]Z".
    /// Base for date related subtypes such as Date, Time, DateTime, DateTimeOffset
    /// </remarks>
    DateTime = 1,

    /// <summary>
    /// Represents text that is displayed.
    /// </summary>
    /// <remarks>
    /// Base for text related sub types, such as PhoneNumber, Email, Url, etc.
    /// </remarks>
    Text = 5, 

    /// <summary>
    /// Represents True/False
    /// </summary>
    Boolean = 6,

    /// <summary>
    /// Represents a Number.
    /// </summary>
    /// <remarks>
    /// Base for number related sub types, such as Integer, Real, Currency, etc.
    /// </remarks>
    Number = 7, 
    
}