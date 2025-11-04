using System.Collections.Generic;

namespace Tiverion.Models.ViewModels.Stats
{
    public class ChartConfig
    {
        public string Type { get; set; } = "line";
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset> Datasets { get; set; } = new();
        public object Options { get; set; } = new { responsive = true, maintainAspectRatio = false };
    }

    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public List<double> Data { get; set; } = new();
        public bool Fill { get; set; } = false;
        public double Tension { get; set; } = 0.3;
        public int? PointRadius { get; set; } = 3;
    }
}