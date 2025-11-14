using System.ComponentModel;
using System.Text.Json.Serialization;

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
        [DisplayName("Температура (°C)")]
        public float TemperatureCels { get; private set; }

        [JsonPropertyName("feelsLikeCels")]
        [DisplayName("Ощущается (°C)")]
        public float FeelsLikeCels { get; private set; }

        [JsonPropertyName("humidity")]
        [DisplayName("Влажность (%)")]
        public int Humidity { get; private set; }

        [JsonPropertyName("condition")]
        [DisplayName("Состояние")]
        public Condition Condition { get; private set; }

        [JsonPropertyName("cloudiness")]
        [DisplayName("Облачность")]
        public Cloudiness Cloudiness { get; private set; }

        [JsonPropertyName("daytime")]
        [DisplayName("Время суток")]
        public Daytime Daytime { get; private set; }

        [JsonPropertyName("windSpeedMpS")]
        [DisplayName("Скорость ветра (м/с)")]
        public float WindSpeedMpS { get; private set; }

        [JsonPropertyName("windGustMpS")]
        [DisplayName("Порыв ветра (м/с)")]
        public float WindGustMpS { get; private set; }

        [JsonPropertyName("windDirection")]
        [DisplayName("Направление ветра")]
        public WindDirection WindDirection { get; private set; }

        [JsonPropertyName("pressureMmHg")]
        [DisplayName("Давление (мм рт. ст.)")]
        public int PressureMmHg { get; private set; }

        [JsonPropertyName("pressurePa")]
        [DisplayName("Давление (Па)")]
        public int PressurePa { get; private set; }

        [JsonPropertyName("visibilityKm")]
        [DisplayName("Видимость (км)")]
        public int VisibilityKm { get; private set; }

        [JsonPropertyName("kpIndex")]
        [DisplayName("KP индекс")]
        public double KpIndex { get; private set; }

        [JsonPropertyName("pollution")]
        [DisplayName("Загрязнение")]
        public PollutionInfo Pollution { get; private set; }
        

        protected CurrentWeather() { }

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
            double kpIndex,
            PollutionInfo pollution)
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
            KpIndex = kpIndex;
            Pollution = pollution;
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
