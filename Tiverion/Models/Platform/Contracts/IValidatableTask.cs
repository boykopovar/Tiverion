

namespace Tiverion.Models.Platform.Contracts;

public interface IValidatableTask : ITask
{
    public void EnsureValidContent();
}