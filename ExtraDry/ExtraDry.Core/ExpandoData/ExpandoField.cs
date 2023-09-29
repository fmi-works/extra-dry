﻿namespace ExtraDry.Core;

/// <summary>
/// Represents a Field in a User Defined Schema, this defines the name, data type, ordering etc.
/// </summary>
public class ExpandoField {

    /// <summary>
    /// A unique slug for the field that is auto-generated on create.
    /// </summary>
    /// <example>external-id</example>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Display Label
    /// </summary>
    public string Label { get; set; } = "label";

    /// <summary>
    /// A description for the field that can present more information than the name.
    /// This may be presented to users in the form of a tooltip or similar.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The data type for the field, these are high level data types like found in JavaScript.
    /// </summary>
    public ExpandoDataType DataType { get; set; }

    /// <summary>
    /// The default value for the field, to be populated by the client when a form is loaded.
    /// If the form is "cancelled", then the default should be reverted.
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// The optional key of the icon to be displayed.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Specifies a short hint that describes the expected value of an input field.
    /// </summary>
    public string? Placeholder { get; set; }

    /// <summary>
    /// Indicates if the value for the field is required.
    /// </summary>
    [Display(Name = "Required")]
    public bool IsRequired { get; set; }

    /// <summary>
    /// Specifies the maximum number of characters that can be entered.
    /// </summary>
    [Display(Name = "Max Text Length")]
    public int MaxLength { get; set; }

    /// <summary>
    /// The available valid values for the field.
    /// </summary>
    [Display(Name = "Valid Values")]
    public List<string>? ValidValues { get; set; }

    /// <summary>
    /// A Minimum value for Integer or Date data types.
    /// </summary>
    public string? RangeMinimum { get; set; }

    /// <summary>
    /// A Maximum value for Integer or Date data types.
    /// </summary>
    public string? RangeMaximum { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how the Data Warehouse should interpret this value.
    /// </summary>
    public WarehouseBehavior WarehouseBehavior { get; set; }

    /// <summary>
    /// Order of the field for display purposes.
    /// </summary>
    public int Order { get; set; }

    /// <inheritdoc cref="ExpandoState"/>
    public ExpandoState State { get; set; } = ExpandoState.Draft;

    public IEnumerable<ValidationResult> ValidateValue(object? value)
    {
        var results = new List<ValidationResult>();

        ValidateDataType(value, ref results);

        if(IsRequired && value == null) {
            results.Add(new ValidationResult($"{Label} is required.", new[] { Label }));
        }

        var stringVal = value?.ToString();
        if(stringVal != null) {
            if(MaxLength > 0 && stringVal.Length > MaxLength) {
                results.Add(new ValidationResult($"{Label} exceeds Maxlength.", new[] { Label }));
            }

            if(ValidValues != null && ValidValues.Any() && !ValidValues!.Contains(stringVal)) {
                results.Add(new ValidationResult($"{Label} does not exist in list of ValidValues.", new[] { Label }));
            }
        }

        return results;
    }

    private void ValidateDataType(object? value, ref List<ValidationResult> results)
    {
        if(value == null) return;

        switch(DataType) {
            case ExpandoDataType.Boolean:
                if(!bool.TryParse(value.ToString(), out var _)) {
                    results.Add(new ValidationResult($"{Label} does not match the DataType set.", new[] { Label }));
                }
                break;
            case ExpandoDataType.DateTime:
            case ExpandoDataType.Date:
            case ExpandoDataType.Time:
                if(!DateTime.TryParse(value.ToString(), out var dateTime)) {
                    results.Add(new ValidationResult($"{Label} does not match the DataType set.", new[] { Label }));
                }
                else {
                    ValidateDate(dateTime, ref results);
                }
                break;
            case ExpandoDataType.Number:
                if(!double.TryParse(value.ToString(), out var number)) {
                    results.Add(new ValidationResult($"{Label} does not match the DataType set.", new[] { Label }));
                }
                else {
                    ValidateNumber(number, ref results);
                }
                break;
        }
    }

    private void ValidateDate(DateTime dateTimeVal, ref List<ValidationResult> results)
    {
        if(DateTime.TryParse(RangeMinimum, out DateTime dtRangeMin) && dateTimeVal < dtRangeMin) {
            results.Add(new ValidationResult($"{Label} does not meet RangeMinimum set.", new[] { Label }));
        }

        if(DateTime.TryParse(RangeMaximum, out DateTime dtRangeMax) && dateTimeVal > dtRangeMax) {
            results.Add(new ValidationResult($"{Label} exceeds RangeMaximum set.", new[] { Label }));
        }
    }

    private void ValidateNumber(double number, ref List<ValidationResult> results)
    {
        if(double.TryParse(RangeMinimum, out double intRangeMin) && number < intRangeMin) {
            results.Add(new ValidationResult($"{Label} does not meet RangeMinimum set.", new[] { Label }));
        }

        if(double.TryParse(RangeMaximum, out double intRangeMax) && number > intRangeMax) {
            results.Add(new ValidationResult($"{Label} exceeds RangeMaximum set.", new[] { Label }));
        }

        if(ValidValues != null && ValidValues.Any()) {
            var intValidValues = ValidValues.ConvertAll(s => double.TryParse(s, out double x) ? x : 0);
            if(!intValidValues.Contains(number)) {
                results.Add(new ValidationResult($"{Label} does not exist in list of ValidValues.", new[] { Label }));
            }
        }
        
    }
}

