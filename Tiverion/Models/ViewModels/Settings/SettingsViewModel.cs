using Tiverion.Models.Platform;
using Tiverion.Models.Platform.Tasks;

namespace Tiverion.Models.ViewModels.Settings;

public class SettingsViewModel
{
    public List<AppUser> Users { get; set; } = new();
    public List<Invitation> Invitations { get; set; } = new();
}