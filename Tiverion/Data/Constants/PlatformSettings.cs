using System.ComponentModel;
using System.Globalization;
using Tiverion.Models.Entities.ServiceEntities;
using System.Reflection;
using Tiverion.Models.Entities.ServiceEntities.Weather;

namespace Tiverion.Data.Constants;

public static class PlatformSettings
{
    public const string PlatformName = "Tiverion";
    public const string PlatformShortName = "Tiverion";
    public static readonly CultureInfo DefaultCulture = new("en-US");
    public static readonly MapPoint DefaultPosition = new(53.9006, 27.5590);

    public static class Urls
    {
        public const string HomePage = "/Home/Index";
        public const string LoginPage = "/Account/Login";
        public const string LogOutPage = "/Account/Logout";
        public const string RegisterPage = "/Account/Register";
        public const string DashboardPage = "/Dashboard/Index";
        public const string StatsPage = "/Stats/Index";
        public const string TasksPage = "/Stats/Tasks";
        public const string SettingsPage = "/Settings/Index";
    }
    
    public static readonly string[] PublicPages =
    {
        "/",
        Urls.HomePage,
        Urls.LoginPage,
        Urls.RegisterPage
    };
    
    public static readonly IReadOnlyList<(string Name, Func<CurrentWeather, object?> Getter)> CurrentWeatherAccessors =
        typeof(CurrentWeather)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
            .Select(p =>
            {
                var displayNameAttr = p.GetCustomAttribute<DisplayNameAttribute>();
                var name = displayNameAttr != null ? displayNameAttr.DisplayName : p.Name;
                Func<CurrentWeather, object?> getter = (cw) => p.GetValue(cw);
                return (name, getter);
            })
            .ToList();
    
}