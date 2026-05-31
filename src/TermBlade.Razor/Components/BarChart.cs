using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A bar chart component supporting vertical/horizontal bars and sparkline mode.
/// </summary>
public sealed class BarChart : RenderableComponentBase<BarChartRenderable>
{
  [Parameter] public IReadOnlyList<double> Data { get; set; } = [];
  [Parameter] public IReadOnlyList<string>? Labels { get; set; }
  [Parameter] public string Orientation { get; set; } = "vertical";
  [Parameter] public bool Sparkline { get; set; }
  [Parameter] public string BarColor { get; set; } = "#58a6ff";
  [Parameter] public string LabelColor { get; set; } = "#8b949e";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public int BarGap { get; set; } = 1;
  [Parameter] public int BarWidth { get; set; } = 3;
  [Parameter] public double? MaxValue { get; set; }

  protected override BarChartRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(BarChartRenderable renderable)
  {
    renderable.Data = Data;
    renderable.Labels = Labels;
    renderable.Orientation = Orientation;
    renderable.Sparkline = Sparkline;
    renderable.BarColor = BarColor;
    renderable.LabelColor = LabelColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.BarGap = BarGap;
    renderable.BarWidth = BarWidth;
    renderable.MaxValue = MaxValue;
  }
}
