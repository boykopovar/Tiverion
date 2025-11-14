using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tiverion.Models.Clients.Contracts;
using Tiverion.Models.Entities.Dto.Converter;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Entities.ServiceEntities.Weather;

namespace Tiverion.Models.Clients.YandexWeather;

public class YaWeatherClient : IWeatherClient<WeatherContent, CurrentWeather>
{
    private const string Platform = "android";
    private const string AppVersion = "25.6.10";
    private const string WeatherApp = $"yandex-weather-{Platform}/{AppVersion}";
    private const string PublicWeatherId = "5b4b5a44-055f-4884-960e-af9e12301e46";
    private const string HostName = "api.weather.yandex.ru";
    private const string BaseUrl = $"https://{HostName}/mobile";
    private const string Device = "corrupted_device_info";

    private static readonly HttpClient Client = new();
    private static readonly JsonSerializerOptions JsonOptions;
    

    static YaWeatherClient()
    {
        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/graphql-response+json"));
        Client.DefaultRequestHeaders.UserAgent.ParseAdd(WeatherApp);
        Client.DefaultRequestHeaders.Add("X-Yandex-Weather-Device", Device);
        Client.DefaultRequestHeaders.Add("X-Yandex-API-Key", PublicWeatherId);
    }
    private YaWeatherClient() { }
    
    public static async Task<WeatherContent> GetWeatherAsync(MapPoint point)
    {
        return await GetWeatherAsync((point.Lat, point.Lon));
    }
    public static async Task<WeatherContent> GetWeatherAsync((double Lat, double Lon) point)
    {
        var (variables, queryText) = YaWeatherQueryBuilder.BuildCombinedWeatherQuery(point.Lat, point.Lon);
        var json = JsonSerializer.Serialize(new { query = queryText, variables });

        using var response = await Client.PostAsync($"{BaseUrl}/graphql/query", new StringContent(json, Encoding.UTF8, "application/json"));
        var jsonWeather = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var weather = JsonToRecord<WeatherContent>.Convert(jsonWeather.RootElement.GetProperty("data").GetProperty("weather"));
        
        if (weather is null) throw new JsonException("returned weather object is null");
        if (weather.Location is null) throw new JsonException("returned weather location is null");
        if (weather.Forecast is null) throw new JsonException("returned weather forecast is null");
        if (weather.Now is null) throw new JsonException("returned weather now is null");
        return weather;
    }
    

    public static async Task<CurrentWeather> GetNowWeatherAsync(MapPoint point)
    {
        return await GetNowWeatherAsync((point.Lat, point.Lon));
    }

    public static async Task<CurrentWeather> GetNowWeatherAsync((double Lat, double Lon) point)
    {
        return (await GetWeatherAsync(point)).Now;
    }
}
