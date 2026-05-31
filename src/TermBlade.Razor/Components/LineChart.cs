using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A line chart component that renders data using Braille characters for sub-cell resolution.
/// </summary>
public sealed class LineChart : RenderableComponentBase<LineChartRenderable>
{
  [Parameter] public IReadOnlyList<double> Data { get; set; } = [];
  [Parameter] public string LineColor { get; set; } = "#58a6ff";
  [Parameter] public string? FillColor { get; set; }
  [Parameter] public string LabelColor { get; set; } = "#8b949e";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public bool ShowYAxis { get; set; } = true;
  [Parameter] public bool ShowXAxis { get; set; } = true;
  [Parameter] public string? Title { get; set; }
  [Parameter] public double? MinValue { get; set; }
  [Parameter] public double? MaxValue { get; set; }

  protected override LineChartRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(LineChartRenderable renderable)
  {
    renderable.Data = Data;
    renderable.LineColor = LineColor;
    renderable.FillColor = FillColor;
    renderable.LabelColor = LabelColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.ShowYAxis = ShowYAxis;
    renderable.ShowXAxis = ShowXAxis;
    renderable.Title = Title;
    renderable.MinValue = MinValue;
    renderable.MaxValue = MaxValue;
  }
}
