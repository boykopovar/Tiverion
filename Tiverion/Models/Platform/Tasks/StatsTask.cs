using System.ComponentModel.DataAnnotations;
using Tiverion.Models.Entities.Dto;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Platform.Contracts;

namespace Tiverion.Models.Platform.Tasks;

/// <summary>
/// Задача сбора статистики.
/// Содержит информацию о географической области,
/// интервале выполнения, необходимых сервисах и дополнительных параметрах.
/// </summary>
public class StatsTask : IPeriodicTask, IValidatableTask
{
    /// <summary>
    /// Уникальный идентификатор задачи (строковый GUID).
    /// </summary>
    [Key]
    [MaxLength(36)]
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Название задачи.
    /// </summary>
    [Required]
    [MaxLength(24)]
    public string Name { get; set; }
    
    /// <summary>
    /// Точка сбора погоды.
    /// </summary>
    public MapPoint Location { get; set; }
    
    /// <summary>
    /// Интервал выполнения задачи.
    /// </summary>
    public TimeSpan Interval { get; set; }
    
    
    /// <summary>
    /// Максимально допустимое время случайного опоздания.
    /// </summary>
    public TimeSpan MaxDeviation { get; set; }
    

    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Конструктор задачи на основе DTO.
    /// </summary>
    /// <param name="statsTaskDto">DTO с данными новой задачи.</param>
    /// <param name="ownerId">Числовой идентификатор владельца задачи.</param>
    public StatsTask(NewStatsTaskDto statsTaskDto, int ownerId)
    {
        if (statsTaskDto.Id != null) Id = statsTaskDto.Id;
        
        ActivationTime = statsTaskDto.ActivationTime;
        Name = statsTaskDto.Name;
        IsEnabled = statsTaskDto.IsEnabled;
        
        Interval = TimeSpan.FromSeconds(statsTaskDto.IntervalSeconds);
        MaxDeviation = TimeSpan.FromSeconds(statsTaskDto.MaxDeviationSeconds);

        OwnerId = ownerId;
    }
    /// <summary>
    /// Конструктор для EF.
    /// </summary>
    private StatsTask() {}
    
    /// <summary>
    /// Дата и время последнего выполнения задачи (UTC).
    /// </summary>
    public DateTime? LastRun { get; private set; }
    
    /// <summary>
    /// Время активации задачи (UTC). Цикл выполнения задачи начнется не раньше этого времени.
    /// </summary>
    public DateTime ActivationTime { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Обновляет дату и время последнего выполнения задачи (LastRun).
    /// </summary>
    public void UpdateLastRun()
    {
        LastRun = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Числовой идентификатор владельца задачи.
    /// </summary>
    public int OwnerId { get; set; }
    
    public void EnsureValidContent()
    {
        if (Location is null)
            throw new NullReferenceException("Location can not be null.");
        //TODO: проверка пустой на пустую задачу
    }
    
    public void UpdateFromDto(NewStatsTaskDto dto)
    {
        if (dto.Id != null && dto.Id != Id)
            throw new InvalidOperationException("Cannot change Id of existing task");

        Name = dto.Name;
        IsEnabled = dto.IsEnabled;
        
        Interval = TimeSpan.FromSeconds(dto.IntervalSeconds);
        MaxDeviation = TimeSpan.FromSeconds(dto.MaxDeviationSeconds);
        
        ActivationTime = dto.ActivationTime; 
    }

}