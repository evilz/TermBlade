using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a proportional breakdown chart.
/// </summary>
public sealed class BreakdownChart : RenderableComponentBase<BreakdownChartRenderable>
{
  [Parameter] public List<double> Data { get; set; } = [];
  [Parameter] public List<string> Labels { get; set; } = [];

  protected override BreakdownChartRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(BreakdownChartRenderable renderable)
  {
    renderable.Data = Data;
    renderable.Labels = Labels;
  }
}
