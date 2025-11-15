
namespace Tiverion.Models.Entities.ServiceEntities.Weather;

public record WeatherStamp : CurrentWeather
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public double Lat { get; set; }
    public double Lon { get; set; }

    public string TaskId { get; set; } = "";
    
    public WeatherStamp() {}
    
    public WeatherStamp(CurrentWeather weather, MapPoint location, string taskId = "") : base(
        weather.TemperatureCels,
        weather.FeelsLikeCels,
        weather.Humidity,
        weather.Condition,
        weather.Cloudiness,
        weather.Daytime,
        weather.WindSpeedMpS,
        weather.WindGustMpS,
        weather.WindDirection,
        weather.PressureMmHg,
        weather.PressurePa,
        weather.VisibilityKm,
        weather.KpIndex,
        weather.Pollution)
    {
        Lat = location.Lat;
        Lon = location.Lon;
        TaskId = taskId;
    }
}