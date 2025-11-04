using System.ComponentModel.DataAnnotations;

namespace Tiverion.Models.Platform;

public class AppSettings
{
    [Key]
    public required AppSettingName Name { get; set; }
    
    [MaxLength(64)]
    public required int Value { get; set; }
}

public enum AppSettingName
{
    MaxAreaSqMetres,
    MaxOneTimeRequests,
    MinRequestsPerToken
}