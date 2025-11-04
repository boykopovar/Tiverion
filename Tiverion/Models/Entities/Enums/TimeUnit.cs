
using System.ComponentModel.DataAnnotations;

namespace Tiverion.Models.Entities.Enums;

public enum TimeUnit
{
    // [Display(Name = "Секунды")]
    // Seconds = 1,

    [Display(Name = "Минуты")]
    Minutes = 60,

    [Display(Name = "Часы")]
    Hours = 60 * 60,

    [Display(Name = "Дни")]
    Days = 60 * 60 * 24,
}