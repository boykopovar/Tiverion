using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Platform.Contracts;
using Tiverion.Models.Platform.Tasks;

namespace Tiverion.Models.Entities.Dto;

public class FlyRunTask : ITask, IValidatableTask
{
    public string Name { get; set; } = "";

    public string Id { get; set; } = "";
    
    public MapPoint Location { get; set; }

    
    private FlyRunTask() {}
    
    public void EnsureValidContent()
    {
        if (Location is null)
            throw new NullReferenceException("Location can not be null.");
    }

    public FlyRunTask(
        MapPoint location,
        string name = "",
        string id = "")
    {
        Name = name;
        Id = id;
        Location = location;
    }

    public FlyRunTask(FlyRunTaskDto task) : this(
        new MapPoint(task.LocationLat, task.LocationLon),
        "",
        ""){}

    public FlyRunTask(StatsTask statsTask)
        : this(
            statsTask.Location!,
            statsTask.Name,
            statsTask.Id)
    {
    }

}
