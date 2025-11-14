using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tiverion.Models.Clients.Contracts;
using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Entities.ServiceEntities.Weather;
using Tiverion.Models.Platform.Contracts;


namespace Tiverion.Models.Platform.Tasks;

/// <summary>
/// Результат выполнения задачи сбора погодных данных.
/// </summary>
public class StatsTaskResult : ITaskResult
{
    /// <summary>
    /// Идентификатор результата.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(36)]
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Идентификатор задачи, для которой собран результат.
    /// </summary>
    [Required]
    [MaxLength(36)]
    public string TaskId { get; private set; }

    /// <summary>
    /// Время начала выполнения задачи (UTC).
    /// </summary>
    public DateTime TaskStartTime { get; private set; }

    /// <summary>
    /// Время окончания выполнения задачи (UTC).
    /// </summary>
    public DateTime TaskEndTime { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Статус выполнения задачи.
    /// </summary>
    public StatsTaskStatus Status { get; private set; }

    /// <summary>
    /// Исключения, возникшие при выполнении задачи.
    /// </summary>
    public List<string> Errors { get; private set; }
    
    
    [NotMapped]
    public WeatherStamp? Weather { get; private set; }
    

    public StatsTaskResult(
        string taskId,
        DateTime taskStartTime,
        StatsTaskStatus status,
        List<string> errors,
        WeatherStamp? weather
        )
    {
        TaskId = taskId;
        TaskStartTime = taskStartTime;
        Status = status;
        Errors = errors;
        Weather = weather;
    }
    
    private StatsTaskResult() {}
}

