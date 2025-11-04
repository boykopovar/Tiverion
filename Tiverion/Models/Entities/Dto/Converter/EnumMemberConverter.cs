namespace Tiverion.Models.Entities.Dto.Converter;

using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Factory for JSON converters that handle enums with custom string values.
/// </summary>
public class EnumMemberConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Checks if the type is an enum.
    /// </summary>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    /// <summary>
    /// Creates a converter for the given enum type.
    /// </summary>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(EnumMemberConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

/// <summary>
/// JSON converter for enums that uses the EnumMember attribute for custom string values.
/// </summary>
/// <typeparam name="T">Enum type</typeparam>
public class EnumMemberConverter<T> : JsonConverter<T> where T : struct, Enum
{
    private readonly Dictionary<string, T> _map;

    /// <summary>
    /// Builds a lookup from enum string values and EnumMember attributes.
    /// </summary>
    public EnumMemberConverter()
    {
        _map = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        foreach (T val in Enum.GetValues(typeof(T)))
        {
            var name = val.ToString();
            var m = typeof(T).GetMember(name)[0];
            var em = m.GetCustomAttribute<EnumMemberAttribute>();
            var key = em?.Value ?? name;
            _map[key] = val;

            _map[name] = val;
            _map[name.Replace("_", "")] = val;
            _map[name.Replace("_", "-")] = val;
        }
    }

    /// <summary>
    /// Reads enum value from JSON string or number.
    /// </summary>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (s != null)
            {
                if (_map.TryGetValue(s, out var v)) return v;
                var norm = s.Replace("-", "").Replace(" ", "").Replace("_", "");
                if (_map.TryGetValue(norm, out v)) return v;
            }
            throw new JsonException($"Unknown enum value '{s}' for {typeof(T)}");
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var i))
            return (T)Enum.ToObject(typeof(T), i);

        throw new JsonException();
    }

    /// <summary>
    /// Writes enum value to JSON string using EnumMember if present.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var name = value.ToString();
        var m = typeof(T).GetMember(name)[0];
        var em = m.GetCustomAttribute<EnumMemberAttribute>();
        writer.WriteStringValue(em?.Value ?? name);
    }
}
