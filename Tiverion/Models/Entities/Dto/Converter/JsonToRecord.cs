using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

using Tiverion.Models.Entities.Enums;

namespace Tiverion.Models.Entities.Dto.Converter;

public class JsonToRecord<T>
{
    private static readonly JsonSerializerOptions JsonOptions;

    static JsonToRecord()
    {
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
        JsonOptions.Converters.Add(new EnumMemberConverterFactory());
        JsonOptions.Converters.Add(new MapPointConverterFactory());
    }

    public static T Convert(JsonDocument jsonDocument, string? propertyName = null, string? subName = null, string? subSubName = null)
    {
        if (subName is not null && propertyName is null)throw new ArgumentException("subPropertyName cannot be provided without propertyName.");
        if (subSubName is not null && subName is null) throw new ArgumentException("subSubName cannot be provided without subName.");
        
        JsonElement targetElement;
        
        if (propertyName is null)
        {
            targetElement = jsonDocument.RootElement;
        }
        else
        {
            if (!jsonDocument.RootElement.TryGetProperty(propertyName, out targetElement) ||
                (subName is not null && !targetElement.TryGetProperty(subName, out targetElement)))
            {
                throw new JsonException($"{propertyName} not found");
            }
        }
        return Convert(subSubName != null && targetElement.TryGetProperty(subSubName, out var el) ? el : targetElement);
    }
    public static T Convert(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind == JsonValueKind.Array)
        {
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
            {
                var coll = JsonSerializer.Deserialize<T>(jsonElement, JsonOptions)
                           ?? throw new JsonException("Deserialized collection is null");
                return coll;
            }

            if (jsonElement.GetArrayLength() == 0)
                throw new JsonException("Expected object but got empty array.");
            
            return Convert(jsonElement[0]);
        }
        
        if (jsonElement.ValueKind != JsonValueKind.Object)
            throw new JsonException($"Expected JSON object or array but got {jsonElement.ValueKind}");
        
        var result = JsonSerializer.Deserialize<T>(jsonElement, JsonOptions) ?? throw new JsonException("result is null");
        
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.GetCustomAttribute<JsonIgnoreAttribute>() != null) continue;

            var propType = prop.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;
            
            if (underlyingType.IsValueType && Nullable.GetUnderlyingType(propType) == null)
            {
                var value = prop.GetValue(result);
                var defaultValue = Activator.CreateInstance(propType);
                if (value == null || value.Equals(defaultValue))
                {
                    var propName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name;
                    if (!jsonElement.TryGetProperty(propName, out var _))
                    {
                        throw new JsonException($"Missing value for non-nullable property '{propName}'");
                    }
                }
            }
        }

        return result;
    }

    public static async Task<T> Convert(HttpResponseMessage httpResponse)
    {
        return await Convert(await httpResponse.Content.ReadAsStreamAsync());
    }

    public static async Task<T> Convert(Stream streamJson)
    {
        using var doc = await JsonDocument.ParseAsync(streamJson);
        return Convert(doc.RootElement);
    }
    
}

