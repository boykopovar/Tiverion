using System.ComponentModel.DataAnnotations;
using Tiverion.Models.Entities.Enums;

namespace Tiverion.Models.Platform;

public class Invitation
{
    [Key]
    [MaxLength(36)]
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    
    public int CreatedByUserId { get; private set; }

    public DateTime? ActivatedAt { get; set; } = null;
    
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; private set; } = null;
    
    public UserRole Role { get; set; } = UserRole.User;
    
    [Required]
    [MaxLength(24)]
    public required string Title { get; set; }
    
    [MaxLength(36)]
    public string? Message { get; set; } = null;
}