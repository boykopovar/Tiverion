using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Context;
using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Platform;
using Tiverion.Models.ViewModels.Settings;

namespace Tiverion.Controllers;

public class SettingsController : Controller
{
    private readonly TiverionDbContext _db;

    public SettingsController(TiverionDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _db.Users.ToListAsync();
        var invitations = await _db.Invitations.ToListAsync();
        return View(new SettingsViewModel
        {
            Users = users,
            Invitations = invitations
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateInvitation(string title, string? message, int role)
    {
        var inv = new Invitation
        {
            Title = title,
            Message = message,
            Role = (UserRole)role
        };
        _db.Invitations.Add(inv);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteInvitation(string id)
    {
        var inv = await _db.Invitations.FirstOrDefaultAsync(i => i.Id == id);
        if (inv != null)
        {
            _db.Invitations.Remove(inv);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}