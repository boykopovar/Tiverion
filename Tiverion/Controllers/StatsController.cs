using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Context;
using Tiverion.Models.Entities.Dto;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Platform;
using Tiverion.Models.Platform.Tasks;
using Tiverion.Models.ViewModels.Stats;

namespace Tiverion.Controllers;

public class StatsController : Controller
{
    private readonly TiverionDbContext _db;
    private readonly TiverionCacheContext _cache;
    private readonly TaskEventHub<StatsTask> _eventHub;

    public StatsController(TiverionDbContext db, TiverionCacheContext cache, TaskEventHub<StatsTask> eventHub)
    {
        _db = db;
        _cache = cache;
        _eventHub = eventHub;
    }
    
    public async Task<(IActionResult? action, AppUser? user)> ConfirmUser(ClaimsPrincipal webUser)
    {
        var username = webUser.Identity?.Name;
        if (username is null) return (RedirectToAction("Login", "Account"), null);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return (RedirectToAction("Login", "Account"), null);
        
        return (null, user);
    }
    
    public async Task<IActionResult> Index()
    {
        var actionAndUser = await ConfirmUser(User);
        if (actionAndUser.action is not null) return actionAndUser.action;

        // var model = new StatsTasksViewModel(
        //     await _db.StatsTasks.ToListAsync(),
        //     actionAndUser.user!.LastPosition
        // );

        // return View("Tasks", model);
        var model = new StatsViewModel
        {
            WeatherStamps = _cache.WeatherStamps
        };
        return View(model);
    }
    
    public async Task<IActionResult> Tasks()
    {
        var actionAndUser = await ConfirmUser(User);
        if (actionAndUser.action is not null) return actionAndUser.action;

        return View(new StatsTasksViewModel(
            await _db.StatsTasks.ToListAsync(),
            actionAndUser.user!.LastPosition
        ));
    }

    public async Task<IActionResult> OpenTask(string? taskId)
    {
        var actionAndUser = await ConfirmUser(User);
        if (actionAndUser.action is not null) return actionAndUser.action;

        var tasks = await _db.StatsTasks.ToListAsync();
        //TODO: проверить наличие taskId в tasks
        var model = new StatsTasksViewModel(tasks, actionAndUser.user.LastPosition, taskId);
        return View(model);
    }

    

    private void _UpdateUserLocationNoSave(AppUser user, StatsTask task)
    {
        user.LastPosition = new MapPoint(task.Location.Lat, task.Location.Lon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTask(NewStatsTaskDto newTask)
    {
        var actionAndUser = await ConfirmUser(User);
        if (actionAndUser.action is not null) return actionAndUser.action;
        var user = actionAndUser.user;
        
        try
        {
            bool isUpdate = !string.IsNullOrEmpty(newTask.Id);
            StatsTask? task;
            if (isUpdate)
            {
                task = await _db.StatsTasks
                    .FirstOrDefaultAsync(t => t.Id == newTask.Id);
                if (task is null) throw new KeyNotFoundException($"Task {newTask.Id} not found");
                task.UpdateFromDto(newTask);
            }
            else
            {
                task = new StatsTask(newTask, user!.Id);
                await _db.StatsTasks.AddAsync(task);
            }
            
            task.EnsureValidContent();
            _UpdateUserLocationNoSave(user!, task);
            await _db.SaveChangesAsync();
            
            if(isUpdate)_eventHub.InvokeUpdated(task);
            else _eventHub.InvokeAdded(task);
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            
            var tasks = await _db.StatsTasks.ToListAsync();
            var model = new StatsTasksViewModel(tasks, user!.LastPosition, null);
            return View("OpenTask", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enable(string taskId, bool isEnabled)
    {
        var task = await _db.StatsTasks
            .FirstOrDefaultAsync(t => t.Id == taskId);
        if (task is null)
        {
            TempData["Error"] = "Задача не найдена!";
            return RedirectToAction("Tasks");
        }

        if (!isEnabled) _eventHub.InvokeDeleted(task);
        
        task.IsEnabled = isEnabled;
        await _db.SaveChangesAsync();
        
        if (isEnabled) _eventHub.InvokeAdded(task);
        
        return RedirectToAction("Tasks");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string taskId)
    {
        var username = User.Identity?.Name;
        if (username is null) return RedirectToAction("Login", "Account");
        
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return RedirectToAction("Login", "Account");
        
        try
        {
            var task = await _db.StatsTasks.FindAsync(taskId);
            if (task is null) throw new KeyNotFoundException($"Task '{taskId}' not found'");
            
            _db.StatsTasks.Remove(task);
            await _db.SaveChangesAsync();
            
            _eventHub.InvokeDeleted(task);
            return RedirectToAction("Tasks");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            
            var tasks = await _db.StatsTasks.ToListAsync();
            var model = new StatsTasksViewModel(tasks, user.LastPosition, null);
            return View("OpenTask", model);
        }
    }
}
