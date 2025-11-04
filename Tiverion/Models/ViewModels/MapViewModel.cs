using System.Net;
using Tiverion.Data.Constants;
using Tiverion.Models.Entities.ServiceEntities;

namespace Tiverion.Models.ViewModels;

public class MapViewModel
{
    public MapPoint? Location { get; set; }
    public string? ServiceToken { get; set; }

    public MapViewModel(
        MapPoint? location,
        string? serviceToken = null)
    {
        Location = location;
        ServiceToken = serviceToken;
    }
}