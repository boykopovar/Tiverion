namespace Tiverion.Models.Clients.YandexWeather
{
    internal static class YaWeatherQueryBuilder
    {
        private const string Lang = "RU";
        private const string TemperatureUnit = "CELSIUS";
        private const string WindSpeedUnit = "METERS_PER_SECOND";
        private const string PressureUnit = "MM_HG";
        private const int DaysLimit = 3;

        public static (Dictionary<string, object> variables, string query) BuildCombinedWeatherQuery(
            double lat,
            double lon,
            string lang = Lang,
            string temperatureUnit = TemperatureUnit,
            string windSpeedUnit = WindSpeedUnit,
            string pressureUnit = PressureUnit,
            int daysLimit = DaysLimit)
        {

            var variables = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["lat"] = lat,
                ["lon"] = lon,
                ["lang"] = lang,
                ["temperatureUnit"] = temperatureUnit,
                ["windSpeedUnit"] = windSpeedUnit,
                ["pressureUnit"] = pressureUnit
            };

            var query =
                $@"query getCombinedWeather($lat: Float!, $lon: Float!, $lang: Language!, $temperatureUnit: TemperatureUnit!, $windSpeedUnit: WindSpeedUnit!, $pressureUnit: PressureUnit!) {{
                  time: serverTimestamp
                  weather: weatherByPoint(request: {{ lat: $lat lon: $lon }}, language: $lang) {{
                    location {{ lat lon }}
                    now {{
                      temperatureCels: temperature(unit: $temperatureUnit)
                      feelsLikeCels: feelsLike(unit: $temperatureUnit)
                      humidity
                      condition
                      cloudiness
                      daytime
                      windSpeedMpS: windSpeed(unit: $windSpeedUnit)
                      windGustMpS: windGust(unit: $windSpeedUnit)
                      windDirection
                      pressureMmHg: pressure(unit: $pressureUnit)
                      pressurePa: pressure(unit: PASCAL)
                      visibilityKm: visibility
                      kpIndex(gScale: false)
                      pollution {{ aqi dominant pm2p5 pm10 no2 so2 o3 co }}
                    }}
                    forecast {{
                      days(limit: {daysLimit.ToString()}) {{
                        sunriseBegin
                        sunrise
                        sunset
                      }}
                    }}
                  }}
                }}";

            return (variables, query);
        }
    }
}
