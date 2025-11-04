using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Context;
using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Platform;

namespace Tiverion.Controllers;

public class AccountController : Controller
{
    private readonly TiverionDbContext _db;

    public AccountController(TiverionDbContext db)
    {
        _db = db;
    }
    
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");

        return View();
    }
    
    public async Task<IActionResult> Register(string? invitationId)
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");

        if (invitationId == null)
        {
            TempData["Error"] = "Регистрация пока недоступна";
            return View();
        }


        var invitation = await _db.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId && i.ActivatedAt == null);
        if (!(invitation is null) && invitation.ExpiresAt >= DateTimeOffset.UtcNow) invitation = null;
        
        if (invitation == null)
        {
            TempData["Error"] = "Регистрация пока недоступна";
            return View();
        }
        ViewData["InvitationId"] = invitationId;
        return View();
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, bool rememberMe)
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");

        await _CreateTestUser();
        
        var hash = HashPassword(password);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hash);
        if (user == null)
        {
            TempData["Error"] = "Неверный логин или пароль";
            return View();
        }

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
            });

        return RedirectToAction("Index", "Home");
    }

    private async Task _CreateTestUser()
    {
        var userName = "tp104";
        var password = "qweRT123";
        
        if (!await _db.Users.AnyAsync(u => u.Username == userName))
        {
            var user = new AppUser
            {
                Username = userName,
                PasswordHash = HashPassword(password), 
                Role = UserRole.User
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }


    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string username, string password, string confirmPassword, string? invitationId)
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");

        if (invitationId == null)
        {
            TempData["Error"] = "Регистрация пока недоступна";
            return View();
        }

        var invitation = await _db.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId && i.ActivatedAt == null);
        if (!(invitation is null) && invitation.ExpiresAt >= DateTimeOffset.UtcNow) invitation = null;
        
        if (invitation == null)
        {
            TempData["Error"] = "Регистрация пока недоступна";
            return View();
        }

        if (await _db.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
        {
            TempData["Error"] = "Пользователь с таким именем уже существует";
            ViewData["InvitationId"] = invitationId;
            return View();
        }

        if (!Regex.IsMatch(username, @"^(?=.*[A-Za-z0-9])[A-Za-z0-9_]{3,40}$"))
        {
            TempData["Error"] = "Имя пользователя некорректное";
            ViewData["InvitationId"] = invitationId;
            return View();
        }

        if (password != confirmPassword || password.Length < 6)
        {
            TempData["Error"] = "Пароли не совпадают или слишком короткие";
            ViewData["InvitationId"] = invitationId;
            return View();
        }

        var user = new AppUser
        {
            Username = username,
            PasswordHash = HashPassword(password)
        };

        _db.Users.Add(user);
        invitation.ActivatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Регистрация прошла успешно. Войдите в систему.";
        return RedirectToAction("Login");
    }

    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
