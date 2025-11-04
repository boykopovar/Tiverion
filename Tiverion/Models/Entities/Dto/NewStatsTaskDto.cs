
namespace Tiverion.Models.Entities.Dto;

public class NewStatsTaskDto
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    
    public double LocationLat { get; set; }
    public double LocationLon { get; set; }
    
    public int IntervalSeconds { get; set; }
    public int MaxDeviationSeconds { get; set; }
    public DateTime ActivationTime { get; set; } = DateTime.UtcNow;
    public bool IsEnabled { get; set; } = true;

}