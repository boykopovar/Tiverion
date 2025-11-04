using System.Globalization;
using Tiverion.Models.Entities.ServiceEntities;

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
        public const string SettingsPage = "/Settings/Index";
    }
    
    public static readonly string[] PublicPages =
    {
        "/",
        Urls.HomePage,
        Urls.LoginPage,
        Urls.RegisterPage
    };
    
}