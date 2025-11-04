using Tiverion.Models.Platform.Contracts;

namespace Tiverion.Models.Platform.Tasks;

public class TaskEventHub<TTask> where TTask : ITask
{
    public event Action<TTask>? OnTaskAdded;
    public event Action<TTask>? OnTaskDeleted;
    public event Action<TTask>? OnTaskUpdated;

    public void InvokeAdded(TTask task) => OnTaskAdded?.Invoke(task);
    public void InvokeDeleted(TTask task) => OnTaskDeleted?.Invoke(task);
    public void InvokeUpdated(TTask task) => OnTaskUpdated?.Invoke(task);
}
