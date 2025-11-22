using System.ComponentModel.DataAnnotations;

namespace Tiverion.Models.Entities.Enums;

public enum TimePeriod
{
    [Display(Name = "Часы")]
    Hour = 1,
    [Display(Name = "Дни")]
    Day = 24,
    [Display(Name = "Недели")]
    Week = 7 * 24,
}