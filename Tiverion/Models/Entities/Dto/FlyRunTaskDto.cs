using System.Data;
using Tiverion.Models.Entities.Enums;
using Tiverion.Data.Constants;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Platform.Contracts;

namespace Tiverion.Models.Entities.Dto;

public class FlyRunTaskDto
{
    public double LocationLat { get; set; }
    public double LocationLon { get; set; }

    public FlyRunTaskDto() {}
}