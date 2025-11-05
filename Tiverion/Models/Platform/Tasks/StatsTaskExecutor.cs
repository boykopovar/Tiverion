using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Constants;
using Tiverion.Models.Clients.Contracts;
using Tiverion.Models.Clients.YandexWeather;
using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Entities.ServiceEntities.Weather;
using Tiverion.Models.Platform.Contracts;
using Tiverion.Models.Entities.Dto;

namespace Tiverion.Models.Platform.Tasks;

public class StatsTaskExecutor : ITaskExecutor<ITask, StatsTaskResult>
{
    private List<string> _exceptions = new();
    private DbSet<AppSettings> _settings;
    private CancellationToken _cancelToken;


    private async Task<int> _GetSetting(AppSettingName setting)
    {
        return await _settings
            .Where(s => s.Name == setting)
            .Select(s => s.Value)
            .FirstAsync();
    }
    
    public async Task<StatsTaskResult> ExecuteAsync(
        ITask task,
        DbSet<AppSettings> settings,
        CancellationToken cancelToken)
    {
        var taskName = "Панельная задача";
        if (!string.IsNullOrEmpty(task.Name)) taskName = task.Name;
        
        Console.WriteLine($"Executing: {taskName}");
        var startTime = DateTime.UtcNow;
        
        _settings = settings;
        _cancelToken = cancelToken;

        var weather = await _GetWeatherAsync(task);
        var status = _exceptions.Any() ? StatsTaskStatus.Error : StatsTaskStatus.Success;
        
        Console.WriteLine($"Completed: {taskName}");
        
        return new StatsTaskResult(
            task.Id,
            startTime,
            status,
            _exceptions,
            weather
        );
    }

    private async Task<CurrentWeather?> _GetWeatherAsync(ITask task)
    {
        try
        {
            return await YaWeatherClient.GetNowWeatherAsync(
                task.Location,
                task.Id
            );
        }
        catch (Exception ex){_exceptions.Add(ex.ToString());}

        return null;
    }
}