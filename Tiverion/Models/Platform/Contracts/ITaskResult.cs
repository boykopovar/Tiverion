using System.Collections.Immutable;
using Tiverion.Models.Clients.Contracts;

namespace Tiverion.Models.Platform.Contracts;


/// <summary>
/// Результат выполнения задачи.
/// </summary>
public interface ITaskResult
{
    public string Id { get; }
    public string TaskId { get; }

    /// <summary>
    /// Время начала выполнения задачи (UTC).
    /// </summary>
    public DateTime TaskStartTime { get; }

    /// <summary>
    /// Время окончания выполнения задачи (UTC).
    /// </summary>
    public DateTime TaskEndTime { get; }
    

    /// <summary>
    /// Исключения, возникшие при выполнении задачи.
    /// </summary>
    public List<string> Errors { get; }
    
}