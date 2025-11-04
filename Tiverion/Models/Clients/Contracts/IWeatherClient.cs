using Tiverion.Models.Entities.ServiceEntities;

namespace Tiverion.Models.Clients.Contracts;

public interface IWeatherClient<TWeather, TNow>
{
    static abstract Task<TNow> GetNowWeatherAsync(MapPoint point);
    static abstract Task<TWeather> GetWeatherAsync(MapPoint point);
}