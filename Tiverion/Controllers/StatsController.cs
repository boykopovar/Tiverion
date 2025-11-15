using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Context;
using Tiverion.Models.Entities.Dto;
using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Entities.ServiceEntities.Weather;
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
    
    
    public async Task<IActionResult> Analysis()
    {
        var actionAndUser = await ConfirmUser(User);
        if (actionAndUser.action is not null) return actionAndUser.action;
        
        return View(new AnalysisRangeDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Analysis(AnalysisRangeDto analysisDto)
    {
        var actionAndUser = await ConfirmUser(User);
        if (actionAndUser.action is not null) return actionAndUser.action;
        
        var input = analysisDto.Input;
        if (input is null)
        {
            TempData["Error"] = "Данные не заполнены!";
            return RedirectToAction("Analysis");
        }
        
        var query = _cache.WeatherStamps
            .AsQueryable();
        
        if (input.FromDate.HasValue) query = query
            .Where(w => w.Timestamp >= input.FromDate.Value);
        if (input.ToDate.HasValue) query = query
            .Where(w => w.Timestamp <= input.ToDate.Value);
        
        var properties = (input.NumericRanges?.Keys ?? Enumerable.Empty<string>())
            .Concat(input.EnumRanges?.Keys ?? Enumerable.Empty<string>())
            .Select(k => typeof(CurrentWeather).GetProperty(k))
            .Where(p => p is not null)
            .Select(p => p!)
            .ToList();
        
        if (!properties.Any()) return View(new AnalysisRangeDto { Input = input, ResultPercent = 0 });

        List<WeatherStamp> stamps;
        if(analysisDto.ByAverage)
            stamps = await _GetAverageByInterval(query, properties, analysisDto.SpanForAverageHours);
        else
            stamps = await query.ToListAsync();

        int countMatching = 0;

        foreach (var w in stamps)
        {
            bool matches = true;

            foreach (var prop in properties)
            {
                if (!matches) break;

                var name = prop.Name;

                if (input.NumericRanges != null && input.NumericRanges.TryGetValue(name, out var numericRange))
                {
                    var val = Convert.ToDouble(prop.GetValue(w));
                    if (numericRange.From.HasValue && val < numericRange.From.Value ||
                        numericRange.To.HasValue && val > numericRange.To.Value)
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches && input.EnumRanges != null && input.EnumRanges.TryGetValue(name, out var enumRange))
                {
                    var val = Convert.ToInt32(prop.GetValue(w));
                    if (enumRange.From.HasValue && val < enumRange.From.Value ||
                        enumRange.To.HasValue && val > enumRange.To.Value)
                    {
                        matches = false;
                        break;
                    }
                }
            }

            if (matches) countMatching++;
        }


        double percent = stamps.Count == 0 ? 0 : (countMatching * 100.0 / stamps.Count);
        analysisDto.ResultPercent = percent;
        
        return View(analysisDto);
    }

    public async Task<List<WeatherStamp>> _GetAverageByInterval(
        IQueryable<WeatherStamp> query,
        List<PropertyInfo> properties,
        int spanHours = 24)
    {
        var list = await query.ToListAsync();
        var result = new List<WeatherStamp>();

        if (!list.Any())
            return result;

        list = list
            .OrderBy(s => s.Timestamp)
            .ToList();
        
        var start = list.First().Timestamp;
        var end = list.Last().Timestamp;

        for (var intervalStart = start; intervalStart <= end; intervalStart = intervalStart.AddHours(spanHours))
        {
            var intervalEnd = intervalStart.AddHours(spanHours);
            var intervalGroup = list
                .Where(w => w.Timestamp >= intervalStart && w.Timestamp < intervalEnd)
                .ToList();

            if (!intervalGroup.Any())
                continue;

            var avgStamp = new WeatherStamp();
            avgStamp.Timestamp = intervalStart;

            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsEnum)
                {
                    var values = intervalGroup.Select(w => (int)prop.GetValue(w)!).ToList();
                    if (values.Any())
                    {
                        var mode = values
                            .GroupBy(v => v)
                            .OrderByDescending(g => g.Count())
                            .First().Key;

                        prop.SetValue(avgStamp, Enum.ToObject(prop.PropertyType, mode));
                    }
                }
                else
                {
                    var values = intervalGroup
                        .Select(w => Convert.ToDouble(prop.GetValue(w)))
                        .ToList();

                    if (values.Any())
                        prop.SetValue(avgStamp, Convert.ChangeType(values.Average(), prop.PropertyType));
                }
            }

            result.Add(avgStamp);
        }

        return result;
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
