using Microsoft.EntityFrameworkCore;
using Tiverion.Models.Clients.Contracts;
using Tiverion.Models.Entities.ServiceEntities;
using Tiverion.Models.Entities.ServiceEntities.Weather;
using Tiverion.Models.Platform;
using Tiverion.Models.Platform.Tasks;

namespace Tiverion.Data.Context;

public class TiverionCacheContext : DbContext
{
    public TiverionCacheContext(DbContextOptions<TiverionCacheContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<WeatherStamp> WeatherStamps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<WeatherStamp>(entity =>
        {
            entity.OwnsOne(cw => cw.Pollution);
            entity.HasKey(cw => new {cw.Timestamp, cw.Lat, cw.Lon});
        });
    }
}
