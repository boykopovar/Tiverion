using System.ComponentModel.DataAnnotations;

namespace Tiverion.Models.Entities.Enums;

public enum TimePeriod
{
    [Display(Name = "Часы")]
    [CountForm(Form1 = "час", Form2 = "часа", Form3 = "часов")]
    Hour = 1,
    
    [Display(Name = "Дни")]
    [CountForm(Form1 = "день", Form2 = "дня", Form3 = "дней")]
    Day = 24,
    
    [Display(Name = "Недели")]
    [CountForm(Form1 = "неделя", Form2 = "недели", Form3 = "неделей")]
    Week = 7 * 24,
}

public class CountForm : Attribute
{
    public required string Form1 { get; set; }
    public required string Form2 { get; set; }
    public required string Form3 { get; set; }

    public string GetWordForm(int count)
    {
        if (count > 9)
        {
            var last2 = count % 100;
            if (last2 > 11 && last2 < 14) return Form1;
        }

        var last = count % 10;
        if(last == 1) return Form1;
        if(last == 2 || last == 3 || last == 4) return Form2;
        return Form3;
    }
}