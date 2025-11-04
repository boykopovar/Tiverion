using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Platform.Tasks;

namespace Tiverion.Models.ViewModels.Stats;

public class StatsTasksViewModel
{
    public List<StatsTask> ExistingTasks { get; set; }
    public MapPoint? LastPosition { get; set; }
    
    public string? TargetTask { get; set; }
    
    public string? MapServiceToken { get; set; }

    public StatsTasksViewModel(
        List<StatsTask> existingTasks,
        MapPoint? lastPosition,
        string? targetTask = null,
        string? mapServiceToken = null)
    {
        ExistingTasks = existingTasks;
        LastPosition = lastPosition;
        TargetTask = targetTask;
        MapServiceToken = mapServiceToken;
    }
}