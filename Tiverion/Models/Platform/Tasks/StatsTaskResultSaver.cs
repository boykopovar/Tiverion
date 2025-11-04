using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Context;
using Tiverion.Models.Platform.Contracts;

namespace Tiverion.Models.Platform.Tasks;

public class StatsTaskResultSaver : ITaskResultSaver<StatsTaskResult>
{
    public async Task<bool> SaveTaskResultAsync(TiverionDbContext db, TiverionCacheContext cache, StatsTaskResult result, CancellationToken ct)
    {
        var dbTask = await db.StatsTasks.SingleOrDefaultAsync(t => t.Id == result.TaskId, ct);
        if (dbTask != null) dbTask.UpdateLastRun();
        await db.StatsTaskResults.AddAsync(result, ct);

        try
        {
            if (result.Weather != null) await cache.WeatherStamps.AddAsync(result.Weather, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving weather: {ex.Message}");
        }

        await db.SaveChangesAsync(ct);
        await cache.SaveChangesAsync(ct);
        return true;
    }
}