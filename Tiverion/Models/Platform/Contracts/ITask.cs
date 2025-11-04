using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Entities.ServiceEntities;

namespace Tiverion.Models.Platform.Contracts;

public interface ITask
{
    public string Name { get; set; }
    public string Id { get; }

    public MapPoint Location { get; }
}