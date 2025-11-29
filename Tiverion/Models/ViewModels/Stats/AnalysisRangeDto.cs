using Tiverion.Models.Entities.Enums;

namespace Tiverion.Models.ViewModels.Stats;

public class AnalysisRangeDto
{
    public WeatherConditionRangeDto? Input { get; set; }
    public bool ByAverage { get; set; } = true;
    public int SpanForAverageHours { get; set; } = 24;
    public double? ResultPercent { get; set; } = null;
    public double Percent { get; set; } = 0;

}


public class WeatherConditionRangeDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Dictionary<string, NumericRange>? NumericRanges { get; set; } = new();
    public Dictionary<string, NumericRange>? EnumRanges { get; set; } = new();
    public int NumberOfTrials {get; set; } = -1;
    public int AmountSuccess { get; set; } = -1;
    public TimePeriod Period { get; set; } = TimePeriod.Day;
}

public class NumericRange
{
    public double? From { get; set; }
    public double? To { get; set; }
}


public class GeometricAnalysisDto
{
    public WeatherConditionRangeDto? Input { get; set; }
    public bool ByAverage { get; set; } = true;
    public int SpanForAverageHours { get; set; } = 24;
    
    public int NumberOfSteps { get; set; } = 10;

    public GeometricResult? Result { get; set; }
}

public class GeometricResult
{
    public double SuccessProbabilityPercent { get; set; }  // p × 100
    public double ExpectedTrials { get; set; }   // 1/p  (смещённое, T ≥ 1)
    public double ExpectedFailures { get; set; } // (1-p)/p
    public double ExpectedHours { get; set; }   // ExpectedTrials × шаг в часах
    public double ProbabilityExactlyAtK { get; set; } // P(T = k) × 100
    public double ProbabilityWithinK { get; set; } // P(T ≤ k) × 100
    public int TotalIntervals { get; set; }
    public int SuccessCount { get; set; }
}
