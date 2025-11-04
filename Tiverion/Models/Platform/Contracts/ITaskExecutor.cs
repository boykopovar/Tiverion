
using Microsoft.EntityFrameworkCore;
using Tiverion.Models.Entities.ServiceEntities;

namespace Tiverion.Models.Platform.Contracts;

public interface ITaskExecutor<TTask, TResult>
    where TTask : ITask
    where TResult : ITaskResult
{
    Task<TResult> ExecuteAsync(
        TTask task,
        DbSet<AppSettings> settings,
        CancellationToken ct);
}