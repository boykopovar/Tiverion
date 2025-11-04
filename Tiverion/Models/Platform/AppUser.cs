using System.ComponentModel.DataAnnotations;
using Tiverion.Models.Entities.Enums;
using Tiverion.Models.Entities.ServiceEntities;

namespace Tiverion.Models.Platform;

public class AppUser
{
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(50)]
    public required string Username { get; set; }

    [Required]
    [MaxLength(64)]
    public required string PasswordHash { get; set; }
    
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public bool IsBanned { get; set; } = false;
    public DateTime? LastBannedAt { get; private set; } = null;
    
    public MapPoint? LastPosition { get; set; } = null;
    
    // [MaxLength(64)]
    // public string? MapServiceToken { get; set; }
    
    public bool CanCreateInvites { get; private set; } = false;
    public bool CanEditInvites { get; private set; } = false;
    public bool CanSeeInvites { get; private set; } = false;
    public bool CanSeeUsers { get; private set; } = false;
    public bool CanMenageUsers { get; private set; } = false;
}