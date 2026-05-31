using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A time-series line chart component for plotting timestamped data points.
/// </summary>
public sealed class TimeSeriesLineChart : RenderableComponentBase<TimeSeriesLineChartRenderable>
{
  [Parameter] public IReadOnlyList<TimeSeriesPoint> Data { get; set; } = [];
  [Parameter] public string LineColor { get; set; } = "#58a6ff";
  [Parameter] public string? FillColor { get; set; }
  [Parameter] public string LabelColor { get; set; } = "#8b949e";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public bool ShowYAxis { get; set; } = true;
  [Parameter] public bool ShowXAxis { get; set; } = true;
  [Parameter] public string? Title { get; set; }
  [Parameter] public string TimeFormat { get; set; } = "HH:mm";
  [Parameter] public double? MinValue { get; set; }
  [Parameter] public double? MaxValue { get; set; }

  protected override TimeSeriesLineChartRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(TimeSeriesLineChartRenderable renderable)
  {
    renderable.Data = Data;
    renderable.LineColor = LineColor;
    renderable.FillColor = FillColor;
    renderable.LabelColor = LabelColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.ShowYAxis = ShowYAxis;
    renderable.ShowXAxis = ShowXAxis;
    renderable.Title = Title;
    renderable.TimeFormat = TimeFormat;
    renderable.MinValue = MinValue;
    renderable.MaxValue = MaxValue;
  }
}
