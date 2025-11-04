using Microsoft.EntityFrameworkCore;
using Tiverion.Models.Clients.Contracts;
using Tiverion.Models.Entities.ServiceEntities.Weather;

namespace Tiverion.Models.ViewModels.Stats;

public class StatsViewModel
{
    public required DbSet<CurrentWeather> WeatherStamps { get; set; }
}