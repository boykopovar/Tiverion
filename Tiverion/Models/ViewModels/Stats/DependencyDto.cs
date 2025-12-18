namespace Tiverion.Models.ViewModels.Stats;

/// <summary>
/// Главный DTO для анализа зависимостей между погодными параметрами.
/// Содержит входные параметры анализа и результаты обнаруженных правил.
/// </summary>
public class DependencyAnalysisDto
{
    /// <summary>
    /// Параметры анализа: диапазон дат, минимальная уверенность, максимальное число условий.
    /// </summary>
    public DependencyAnalysisInputDto? Input { get; set; } = null;

    /// <summary>
    /// Список обнаруженных зависимостей, удовлетворяющих условиям входного запроса.
    /// </summary>
    public List<DependencyRule> Results { get; set; } = new();
}

/// <summary>
/// Входные параметры анализа зависимостей.
/// </summary>
public class DependencyAnalysisInputDto
{
    /// <summary>
    /// Начало периода анализа.
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Конец периода анализа.
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Минимальная уверенность (%) для того, чтобы правило считалось значимым.
    /// Значение по умолчанию: 51%.
    /// </summary>
    public double MinConfidencePercent { get; set; } = 51;

    /// <summary>
    /// Максимальное число параметров в условии зависимости.
    /// </summary>
    public int MaxConditionParameters { get; set; } = 2;
    
    public int StepSizeHours { get; set; } = 6;
    public int MinDeltaPercent { get; set; } = 30;
}

/// <summary>
/// Результат одного правила зависимости: набор условий и исход.
/// </summary>
public class DependencyRule
{
    /// <summary>
    /// Список условий (левая часть правила, A).
    /// </summary>
    public List<DependencyDto> Conditions { get; set; } = new();

    /// <summary>
    /// Исход события (правая часть правила, B).
    /// </summary>
    public DependencyDto Outcome { get; set; } = new();

    /// <summary>
    /// Уверенность правила в процентах: вероятность наступления исхода при выполнении условий.
    /// Формула: Confidence = N(A ∩ B) / N(A) × 100%
    /// </summary>
    public double ConfidencePercent { get; set; }

    /// <summary>
    /// Поддержка правила в процентах: доля наблюдений, где произошло A ∩ B.
    /// Формула: SupportPercent = N(A ∩ B) / N × 100%
    /// </summary>
    public double SupportPercent { get; set; }

    /// <summary>
    /// Lift: насколько выполнение условия увеличивает вероятность исхода.
    /// Формула: Lift = P(B|A) / P(B) = (N(A ∩ B)/N(A)) / (N(B)/N)
    /// </summary>
    public double Lift { get; set; }

    /// <summary>
    /// Абсолютное количество наблюдений, где произошло A ∩ B.
    /// </summary>
    public int SupportCount { get; set; }
}

/// <summary>
/// Описание одного условия или исхода зависимости.
/// </summary>
public class DependencyDto
{
    /// <summary>
    /// Параметр погоды, например "Давление", "Ветер", "Облачность".
    /// </summary>
    public string Parameter { get; set; } = "";

    /// <summary>
    /// Оператор сравнения.
    /// </summary>
    public DependencyOperator Operator { get; set; }

    /// <summary>
    /// Значение параметра для сравнения (например, 5 мм рт. ст., 12 м/с).
    /// </summary>
    public double Value { get; set; }
    public double ValuePercent { get; set; }

    /// <summary>
    /// Смещение по времени (часы) относительно текущего события.
    /// </summary>
    public int? TimeOffsetHours { get; set; }
}

/// <summary>
/// Операторы сравнения для определения условия зависимости.
/// </summary>
public enum DependencyOperator
{
    Greater,        // >
    Less,           // <
    ChangedTo       // изменение состояния enum (например, изменение статуса облачности)
}
