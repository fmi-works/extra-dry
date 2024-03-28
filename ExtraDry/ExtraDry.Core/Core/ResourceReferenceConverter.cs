using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace ExtraDry.Core;

/// <summary>
/// A JSON converter that will serialize a resource that implement IResourceIdentifiers into a 
/// shorter ResourceReference for use as part of a DTO.  On Deserialization, the original 
/// object is approximated by using default values and Id references. This is suitable for 
/// properties that are treated as RuleAction.Link.
/// </summary>
public class ResourceReferenceConverter<T> : JsonConverter<T> where T : IResourceIdentifiers
{

    /// <inheritdoc cref="JsonConverter{T}.ReadAsPropertyName(ref Utf8JsonReader, Type, JsonSerializerOptions)" />
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var reference = JsonSerializer.Deserialize<ResourceReference<T>>(ref reader, options);
        if(reference == null) {
            return default;
        }
        var obj = (T)Activator.CreateInstance(typeof(T), true) ?? throw new DryException(System.Net.HttpStatusCode.InternalServerError,
                "An internal error has occurred and can only be resolved through a support request.",
                $"Unable to create instance of resource '{typeof(T).Name}' from its resource reference.  Ensure the class has a default constructor.");
        foreach(var property in typeof(T).GetProperties()) {
            var defaultValue = property.GetCustomAttribute<DefaultValueAttribute>();
            if(defaultValue != null) {
                property.SetValue(obj, defaultValue.Value, null);
            }
        }
        if(reference.Uuid != Guid.Empty) {
            obj.Uuid = reference.Uuid;
        }
        if(!string.IsNullOrEmpty(reference.Slug)) {
            obj.Slug = reference.Slug ?? string.Empty;
        }
        if(!string.IsNullOrEmpty(reference.Title)) {
            obj.Title = reference.Title ?? string.Empty;
        }
        var validator = new DataValidator();
        var valid = validator.ValidateObject(obj);
        if(!valid) {
            var errorMessage = string.Join("\r\n", validator.Errors.Select(e => $"  * {e.ErrorMessage}"));
            throw new DryException(System.Net.HttpStatusCode.InternalServerError,
                "An internal error has occurred and can only be resolved through a support request.",
                $"Unable to deserialize a valid object of resource '{typeof(T).Name}' from its resource reference.  Ensure that all properties resolve to valid values.  Note that for properties that have `RuleAction.Link`, this value will likely be replaced so a temporary value is acceptable.  For this deserialization case, the use of the DefaultValueAttribute is recommended.\r\n{errorMessage}");
        }
        return obj;
    }

    /// <inheritdoc cref="JsonConverter{T}.Write(Utf8JsonWriter, T, JsonSerializerOptions)" />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (IResourceIdentifiers)value!, options);
    }

}
