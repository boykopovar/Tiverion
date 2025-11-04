using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Constants;
using Tiverion.Data.Context;
using System.Globalization;
using Tiverion.Controllers;
using Tiverion.Models.Platform.Tasks;

using StatsTaskRunner = Tiverion.Models.Platform.Tasks.TasksRunner<
    Tiverion.Models.Platform.Tasks.StatsTaskExecutor,
    Tiverion.Models.Platform.Tasks.StatsTaskResultSaver,
    Tiverion.Models.Platform.Tasks.StatsTask, 
    Tiverion.Models.Platform.Tasks.StatsTaskResult
>;


var builder = WebApplication.CreateBuilder(args);


CultureInfo.DefaultThreadCurrentCulture = PlatformSettings.DefaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = PlatformSettings.DefaultCulture;


builder.Services.AddControllersWithViews();
    builder.Services.AddSingleton<StatsTaskRunner>(sp =>
    {
        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TiverionDbContext>();
        
        return new (scopeFactory, db.StatsTasks);
    });


var mainConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TiverionDbContext>(options =>
    options.UseSqlite(mainConnection));

var cacheConnection = builder.Configuration.GetConnectionString("CacheConnection");
builder.Services.AddDbContext<TiverionCacheContext>(options =>
    options.UseSqlite(cacheConnection));



builder.Services.AddScoped<StatsController>();
builder.Services.AddSingleton<TaskEventHub<StatsTask>>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = PlatformSettings.Urls.LoginPage;
        options.LogoutPath = PlatformSettings.Urls.LogOutPage;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<StatsTaskRunner>();
    var eventHub = scope.ServiceProvider.GetRequiredService<TaskEventHub<StatsTask>>();
    eventHub.OnTaskAdded += runner.Start;
    eventHub.OnTaskDeleted += runner.Stop;
    eventHub.OnTaskUpdated += runner.Update;
}

app.Services.GetRequiredService<StatsTaskRunner>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();


app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    bool isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

    if (!isAuthenticated &&
        !PlatformSettings.PublicPages.Contains(path.Value, StringComparer.OrdinalIgnoreCase) &&
        !path.StartsWithSegments("/css") &&
        !path.StartsWithSegments("/js") &&
        !path.StartsWithSegments("/res") &&
        !path.StartsWithSegments("/images"))
    {
        context.Response.Redirect(PlatformSettings.Urls.LoginPage);
        return;
    }

    await next();
});

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();


