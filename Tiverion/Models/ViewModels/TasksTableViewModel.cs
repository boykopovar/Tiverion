using Tiverion.Models.Platform.Tasks;

namespace Tiverion.Models.ViewModels;

public class TasksTableViewModel
{
    public List<StatsTask> Tasks { get; set; }
    public bool NeedLaunchBtn { get; set; }

    public TasksTableViewModel(
        List<StatsTask> tasks,
        bool needLaunchBtn)
    {
        Tasks = tasks;
        NeedLaunchBtn = needLaunchBtn;
    }
}