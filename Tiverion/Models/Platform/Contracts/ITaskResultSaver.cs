using Microsoft.EntityFrameworkCore;
using Tiverion.Data.Context;

namespace Tiverion.Models.Platform.Contracts;


public interface ITaskResultSaver<TResult>
{
    /// <summary>
    /// Сохраняет результат задачи в базе данных.
    /// </summary>
    /// <param name="db">Контекст основной базы данных.</param>
    /// /// <param name="cache">Контекст базы данных для записи множественных объектов.</param>
    /// <param name="result">Результат выполнения задачи.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Возвращает <c>true</c>, если задача была найдена <c>false</c>, если задача не найдена (неправильное поведение).</returns>
    public Task<bool> SaveTaskResultAsync(TiverionDbContext db, TiverionCacheContext cache, TResult result, CancellationToken ct);
}