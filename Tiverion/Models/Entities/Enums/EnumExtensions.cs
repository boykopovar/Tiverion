using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Tiverion.Models.Entities.Enums;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DisplayAttribute>();
        return attr?.Name ?? value.ToString();
    }

    public static Attribute? GetAttribute<T>(this Enum value) where T : Attribute
    {
        var field = value.GetType().GetField(value.ToString());
        return field?.GetCustomAttribute<T>();
    }

    public static string GetDisplayName(PropertyInfo field)
    {
        var name = field.GetCustomAttribute<DisplayAttribute>();
        return name?.Name ?? field.Name;
    }
}