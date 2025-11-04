using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiverion.Models.Clients.Contracts;

namespace Tiverion.Models.Entities.ServiceEntities.Weather
{
    public record WeatherContent(
        [property: JsonPropertyName("location")] MapPoint Location,
        [property: JsonPropertyName("now")] CurrentWeather Now,
        [property: JsonPropertyName("forecast")] WeatherForecast Forecast
    )
    {
        public WeatherContent() : this(default!, default!, default!) { }
    };

    public record CurrentWeather
    {
        [JsonPropertyName("temperatureCels")]
        public float TemperatureCels { get; private set; }

        [JsonPropertyName("feelsLikeCels")]
        public float FeelsLikeCels { get; private set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; private set; }

        [JsonPropertyName("condition")]
        public Condition Condition { get; private set; }

        [JsonPropertyName("cloudiness")]
        public Cloudiness Cloudiness { get; private set; }

        [JsonPropertyName("daytime")]
        public Daytime Daytime { get; private set; }

        [JsonPropertyName("windSpeedMpS")]
        public float WindSpeedMpS { get; private set; }

        [JsonPropertyName("windGustMpS")]
        public float WindGustMpS { get; private set; }

        [JsonPropertyName("windDirection")]
        public WindDirection WindDirection { get; private set; }

        [JsonPropertyName("pressureMmHg")]
        public int PressureMmHg { get; private set; }

        [JsonPropertyName("pressurePa")]
        public int PressurePa { get; private set; }

        [JsonPropertyName("visibilityKm")]
        public int VisibilityKm { get; private set; }

        [JsonPropertyName("pollution")]
        public PollutionInfo Pollution { get; private set; }

        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
        
        public double Lat { get; set; }
        public double Lon { get; set; }

        private CurrentWeather() { }

        public CurrentWeather(
            float temperatureCels,
            float feelsLikeCels,
            int humidity,
            Condition condition,
            Cloudiness cloudiness,
            Daytime daytime,
            float windSpeedMpS,
            float windGustMpS,
            WindDirection windDirection,
            int pressureMmHg,
            int pressurePa,
            int visibilityKm,
            PollutionInfo pollution,
            double lat,
            double lon)
        {
            TemperatureCels = temperatureCels;
            FeelsLikeCels = feelsLikeCels;
            Humidity = humidity;
            Condition = condition;
            Cloudiness = cloudiness;
            Daytime = daytime;
            WindSpeedMpS = windSpeedMpS;
            WindGustMpS = windGustMpS;
            WindDirection = windDirection;
            PressureMmHg = pressureMmHg;
            PressurePa = pressurePa;
            VisibilityKm = visibilityKm;
            Pollution = pollution;
            lat = lat;
            lon = lon;
        }
    }


    public record PollutionInfo(
        [property: JsonPropertyName("aqi")] int Aqi,
        [property: JsonPropertyName("dominant")] PollutionDominant Dominant,
        [property: JsonPropertyName("pm2p5")] double Pm2p5,
        [property: JsonPropertyName("pm10")] double Pm10,
        [property: JsonPropertyName("no2")] double No2,
        [property: JsonPropertyName("so2")] double So2,
        [property: JsonPropertyName("o3")] double O3,
        [property: JsonPropertyName("co")] double Co
    );

    public record WeatherForecast(
        [property: JsonPropertyName("days")] List<WeatherDayForecast> Days
    );

    public record WeatherDayForecast(
        [property: JsonPropertyName("sunrise")] TimeSpan Sunrise,
        [property: JsonPropertyName("sunset")] TimeSpan Sunset,
        [property: JsonPropertyName("sunriseBegin")] TimeSpan SunriseBegin
    );
}
