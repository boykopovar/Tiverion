using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiverion.Models.Entities.ServiceEntities;

namespace Tiverion.Models.Entities.Dto.Converter
{
    public class MapPointConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(MapPoint)) return true;
            if (typeToConvert.IsAbstract || typeToConvert.IsInterface || typeToConvert.IsValueType) return false;
            var props = typeToConvert.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return props.Any(p => p.PropertyType == typeof(MapPoint));
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(MapPoint))
                return new MapPointConverter();
            Type converterType = typeof(MapPointContainingConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType, options)!;
        }
    }

    internal class MapPointConverter : JsonConverter<MapPoint>
    {
        private static readonly ConstructorInfo MapCtor;
        private static readonly PropertyInfo LatProp;
        private static readonly PropertyInfo LonProp;

        static MapPointConverter()
        {
            MapCtor = typeof(MapPoint).GetConstructor(new[] { typeof(double), typeof(double) })!;
            LatProp = typeof(MapPoint).GetProperty("Lat")!;
            LonProp = typeof(MapPoint).GetProperty("Lon")!;
        }

        public override MapPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();
            double? lat = null;
            double? lon = null;
            reader.Read();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                string propName = reader.GetString()!;
                reader.Read();
                if (propName.Equals("lat", StringComparison.OrdinalIgnoreCase))
                    lat = reader.GetDouble();
                else if (propName.Equals("lon", StringComparison.OrdinalIgnoreCase) || propName.Equals("lng", StringComparison.OrdinalIgnoreCase))
                    lon = reader.GetDouble();
                else
                    reader.Skip();
                reader.Read();
            }
            if (!lat.HasValue || !lon.HasValue)
                throw new JsonException("Missing lat or lon/lng");
            return (MapPoint)MapCtor.Invoke(new object[] { lat.Value, lon.Value });
        }

        public override void Write(Utf8JsonWriter writer, MapPoint value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("lat", (double)LatProp.GetValue(value)!);
            writer.WriteNumber("lon", (double)LonProp.GetValue(value)!);
            writer.WriteEndObject();
        }
    }

    internal class MapPointContainingConverter<T> : JsonConverter<T> where T : new()
    {
        private readonly JsonSerializerOptions _options;
        private static readonly PropertyInfo LatProp;
        private static readonly PropertyInfo LonProp;
        private static readonly ConstructorInfo MapCtor;
        private static readonly PropertyInfo? MapPointProp;

        static MapPointContainingConverter()
        {
            MapCtor = typeof(MapPoint).GetConstructor(new[] { typeof(double), typeof(double) })!;
            LatProp = typeof(MapPoint).GetProperty("Lat")!;
            LonProp = typeof(MapPoint).GetProperty("Lon")!;
            MapPointProp = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(p => p.PropertyType == typeof(MapPoint));
        }

        public MapPointContainingConverter(JsonSerializerOptions options)
        {
            _options = options;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();
            double? lat = null;
            double? lon = null;
            var obj = new T();
            reader.Read();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                string propName = reader.GetString()!;
                reader.Read();
                bool handled = false;

                if (propName.Equals("lat", StringComparison.OrdinalIgnoreCase))
                {
                    lat = reader.GetDouble();
                    handled = true;
                }
                else if (propName.Equals("lon", StringComparison.OrdinalIgnoreCase) || propName.Equals("lng", StringComparison.OrdinalIgnoreCase))
                {
                    lon = reader.GetDouble();
                    handled = true;
                }

                if (!handled)
                {
                    var prop = FindPropertyByJsonName(typeof(T), propName, _options);
                    if (prop != null)
                    {
                        var value = JsonSerializer.Deserialize(ref reader, prop.PropertyType, _options);
                        prop.SetValue(obj, value);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                reader.Read();
            }

            if (lat.HasValue && lon.HasValue && MapPointProp != null)
            {
                var map = (MapPoint)MapCtor.Invoke(new object[] { lat.Value, lon.Value });
                MapPointProp.SetValue(obj, map);
            }

            return obj;
        }
        
        static private PropertyInfo? FindPropertyByJsonName(Type type, string jsonName, JsonSerializerOptions options)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanWrite) continue;
                if (p.GetCustomAttribute<JsonIgnoreAttribute>() != null) continue;

                var jpn = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                if (!string.IsNullOrEmpty(jpn))
                {
                    if (string.Equals(jpn, jsonName, StringComparison.OrdinalIgnoreCase)) return p;
                    continue;
                }

                if (options.PropertyNameCaseInsensitive)
                {
                    if (string.Equals(p.Name, jsonName, StringComparison.OrdinalIgnoreCase)) return p;
                }
                else
                {
                    if (p.Name == jsonName) return p;
                }
                            
                if (options.PropertyNamingPolicy != null)
                {
                    var policyName = options.PropertyNamingPolicy.ConvertName(p.Name);
                    if (string.Equals(policyName, jsonName, StringComparison.OrdinalIgnoreCase)) return p;
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (MapPointProp != null && prop.Name == MapPointProp.Name)
                {
                    var map = prop.GetValue(value) as MapPoint;
                    if (map != null)
                    {
                        writer.WriteNumber("lat", (double)LatProp.GetValue(map)!);
                        writer.WriteNumber("lon", (double)LonProp.GetValue(map)!);
                    }
                    continue;
                }

                var val = prop.GetValue(value);
                if (val == null && _options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull) continue;
                string jsonName = _options.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name;
                writer.WritePropertyName(jsonName);
                JsonSerializer.Serialize(writer, val, prop.PropertyType, _options);
            }
            writer.WriteEndObject();
        }
    }
}
