using Microsoft.EntityFrameworkCore;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Platform;
using Tiverion.Models.Platform.Tasks;


namespace Tiverion.Data.Context;

public class TiverionDbContext : DbContext
{
    public TiverionDbContext(DbContextOptions<TiverionDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    
    public DbSet<StatsTask> StatsTasks { get; set; }
    public DbSet<StatsTaskResult> StatsTaskResults { get; set; }
    
    public DbSet<AppSettings> Settings { get; set; }

    
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Owned<MapPoint>();
        
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.OwnsOne(u => u.LastPosition);
        });

        modelBuilder.Entity<AppSettings>()
            .HasKey(s => s.Name);
        
        modelBuilder.Entity<AppSettings>()
            .HasIndex(s => s.Name);
        
        

        modelBuilder.Entity<AppSettings>()
            .HasData(
                new AppSettings { Name = AppSettingName.MaxAreaSqMetres, Value = 200000 },
                new AppSettings { Name = AppSettingName.MaxOneTimeRequests, Value = 20 },
                new AppSettings { Name = AppSettingName.MinRequestsPerToken, Value = 1 }
            );
        
        modelBuilder.Entity<Invitation>()
            .HasKey(inv => inv.Id);
        
        modelBuilder.Entity<StatsTask>()
            .HasKey(t => t.Id);
        
        modelBuilder.Entity<StatsTask>(entity =>
        {
            entity.OwnsOne(t => t.Location);
        });
        
        modelBuilder.Entity<StatsTaskResult>()
            .HasKey(t => t.Id);
        
        modelBuilder.Entity<StatsTaskResult>()
            .HasIndex(t => t.TaskStartTime);
        
    }


    private string GetSqliteType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type == typeof(int) || type == typeof(long)) return "INTEGER";
        if (type == typeof(bool)) return "INTEGER";
        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "REAL";
        if (type == typeof(string)) return "TEXT";
        if (type == typeof(DateTime)) return "TEXT";
        return "TEXT";
    }

}
