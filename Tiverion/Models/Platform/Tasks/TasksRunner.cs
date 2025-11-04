using System.Collections.Concurrent;
using System.Text.Json;
using Tiverion.Data.Context;
using Tiverion.Models.Platform.Contracts;

namespace Tiverion.Models.Platform.Tasks;

public class TasksRunner<TExecutor, TTaskSaver, TTask, TResult>
    where TExecutor : ITaskExecutor<ITask, TResult>, new()
    where TTaskSaver : ITaskResultSaver<TResult>, new()
    where TTask : IPeriodicTask, IValidatableTask
    where TResult : ITaskResult
{
    //private readonly TExecutor _executor = new();
    private readonly TTaskSaver _saver = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _runningTasks = new();
    
    public TasksRunner(IServiceScopeFactory scopeFactory, IEnumerable<TTask> tasks)
    {
        _scopeFactory = scopeFactory;
        
        // using var scope = _scopeFactory.CreateScope();
        // var db = scope.ServiceProvider.GetRequiredService<VespaDbContext>();
        // var cache = scope.ServiceProvider.GetRequiredService<VespaCacheContext>();

        foreach (var task in tasks)
        {
            if(task.IsEnabled) Start(task);
        }
    }

    public void Start(TTask task)
    {
        task.EnsureValidContent();
        if (_runningTasks.ContainsKey(task.Id)) return;
        
        var cts = new CancellationTokenSource();
        _runningTasks[task.Id] = cts;

        _ = RunPeriodic(task, cts.Token);
    }
    
    public void Stop(TTask task)
    {
        if (_runningTasks.Remove(task.Id, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    private async Task RunPeriodic(TTask task, CancellationToken ct)
    {
        try
        {
            var rand = new Random();
            var deviationSeconds = rand.Next(0, (int)task.MaxDeviation.TotalSeconds + 1);
            
            var now = DateTime.UtcNow;
            var startTime = task.ActivationTime > now
                ? task.ActivationTime + TimeSpan.FromSeconds(deviationSeconds)
                : now;

            
            if(task.LastRun != null && startTime - task.LastRun < task.Interval)
                startTime = (DateTime)task.LastRun + task.Interval;
            
            var delay = startTime - now;
            Console.WriteLine($"Starting task {task.Id} after {delay.TotalSeconds} seconds");
            
            if (delay > TimeSpan.Zero) await Task.Delay(delay, ct);

            using var timer = new PeriodicTimer(task.Interval);
            bool firstTick = true;

            while (true)
            {
                if (!firstTick && !await timer.WaitForNextTickAsync(ct)) break;
                firstTick = false;
                
                var tickDeviation = rand.Next(0, (int)task.MaxDeviation.TotalSeconds + 1);
                if (tickDeviation > 0)
                    await Task.Delay(TimeSpan.FromSeconds(tickDeviation), ct);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TiverionDbContext>();
                var cache = scope.ServiceProvider.GetRequiredService<TiverionCacheContext>();

                TExecutor executor = new();
                var result = await executor.ExecuteAsync(task, db.Settings, ct);
                
                var res = await _saver.SaveTaskResultAsync(db, cache, result, ct);
            }
        }
        catch (OperationCanceledException) { }
        finally { _runningTasks.TryRemove(task.Id, out _); }
    }
    
    public void Update(TTask task)
    {
        if (!_runningTasks.ContainsKey(task.Id)) throw new KeyNotFoundException("Task not found");
        Stop(task);
        Start(task);
    }
}