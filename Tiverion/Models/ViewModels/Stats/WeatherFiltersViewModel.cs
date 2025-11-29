namespace Tiverion.Models.ViewModels.Stats;

public class WeatherFiltersViewModel
{
    public required WeatherConditionRangeDto? Input { get; set; }
    
    public required bool ByAverage { get; set; } = true;
    
    public required int SpanForAverageHours { get; set; } = 24;
}