using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Entities.ServiceEntities.Weather;
using Tiverion.Models.Platform.Tasks;

namespace Tiverion.Models.ViewModels.Dashboard;

public class DashboardViewModel
{
    public MapPoint? LastPosition { get; set; }
    
    public required List<StatsTask> Tasks { get; set; }
    public required List<StatsTaskResult> LastDayResults { get; set; }
    public required List<WeatherStamp> LastDayWeatherStamps { get; set; }
}