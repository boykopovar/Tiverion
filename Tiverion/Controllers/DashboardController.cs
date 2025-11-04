using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Context;
using Tiverion.Models.Entities.Dto;
using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Platform;
using Tiverion.Models.Platform.Contracts;
using Tiverion.Models.Platform.Tasks;
using Tiverion.Models.ViewModels.Dashboard;

namespace Tiverion.Controllers;

public class DashboardController : Controller
{
    private readonly TiverionDbContext _db;
    private readonly TiverionCacheContext _cache;

    public DashboardController(TiverionDbContext db, TiverionCacheContext cache)
    {
        _db = db;
        _cache = cache;
    }
    
    public async Task<(IActionResult? action, AppUser? user)> ConfirmUser(ClaimsPrincipal webUser)
    {
        var username = webUser.Identity?.Name;
        if (username is null) return (RedirectToAction("Login", "Account"), null);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return (RedirectToAction("Login", "Account"), null);
        
        return (null, user);
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        var actionAndUser = await ConfirmUser(User);
        if (actionAndUser.action is not null) return actionAndUser.action;
        var user = actionAndUser.user!;

        var yesterday = DateTime.UtcNow.AddDays(-1);
        
        var model = new DashboardViewModel()
        {
            LastPosition = user.LastPosition,
            Tasks = await _db.StatsTasks.ToListAsync(),
            LastDayResults = await _db.StatsTaskResults
                .Where(r => r.TaskStartTime >= yesterday)
                .ToListAsync(),
            LastDayWeatherStamps = await _cache.WeatherStamps
                .Where(w => w.Timestamp >= yesterday)
                .ToListAsync()
        };
        return View(model);
    }

    private async Task _RunTask(ITask task)
    {
        var executor = new StatsTaskExecutor();
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        
        var result = await executor.ExecuteAsync(task, _db.Settings, ct);
        
        if (result.Status == StatsTaskStatus.Success) TempData["Success"] = "Задача выполнена успешно";
        if (result.Status == StatsTaskStatus.NotCompleted) TempData["Error"] = "Задача не была завершена";
        if (result.Status == StatsTaskStatus.Error)
        {
            var processedErrors = result.Errors.Select(e =>
            {
                if (e.Length > 200)
                {
                    var truncated = e.Substring(0, 200);
                    var index = truncated.IndexOf(" at", StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        truncated = truncated.Substring(0, index);
                    }
                    return truncated.Trim();
                }
                return e;
            }).ToList();

            TempData["Error"] = string.Join("<br/>", processedErrors.Select(e => $"• {System.Net.WebUtility.HtmlEncode(e)}"));
        }
        
        if (result.Status == StatsTaskStatus.Canceled) TempData["Error"] = "Задача была отменена";

        var saver = new StatsTaskResultSaver();
        await saver.SaveTaskResultAsync(_db, _cache, result, ct);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Run(FlyRunTaskDto taskDto)
    {
        var task = new FlyRunTask(taskDto);
        task.EnsureValidContent();

        await _RunTask(task);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunStatsTask(string taskId)
    {
        var task = await _db.StatsTasks
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        if (task is null)
        {
            TempData["Error"] = "RunStatsTask: Задача не найдена";
            return RedirectToAction("Index");
        }
        
        task.EnsureValidContent();
        await _RunTask(task);
        return RedirectToAction("Index");
    }
}