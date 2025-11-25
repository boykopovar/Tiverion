using Tiverion.Models.Entities.Enums;

namespace Tiverion.Models.ViewModels.Stats;

public class AnalysisRangeDto
{
    public WeatherConditionRangeDto? Input { get; set; }
    public bool ByAverage { get; set; } = true;
    public int SpanForAverageHours { get; set; } = 24;
    public double? ResultPercent { get; set; } = null;

}


public class WeatherConditionRangeDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Dictionary<string, NumericRange>? NumericRanges { get; set; } = new();
    public Dictionary<string, NumericRange>? EnumRanges { get; set; } = new();
    public int NumberOfTrials {get; set; } = -1;
    public int AmountSuccess { get; set; } = -1;
    public TimePeriod? Period { get; set; } = TimePeriod.Day;
}

public class NumericRange
{
    public double? From { get; set; }
    public double? To { get; set; }
}
