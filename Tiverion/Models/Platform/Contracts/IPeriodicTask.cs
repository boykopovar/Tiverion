

namespace Tiverion.Models.Platform.Contracts;

public interface IPeriodicTask : ITask
{
    TimeSpan Interval { get; }
    TimeSpan MaxDeviation { get; }
    DateTime? LastRun { get; }
    void UpdateLastRun();
    public DateTime ActivationTime { get; }
    
    public bool IsEnabled { get; }
}