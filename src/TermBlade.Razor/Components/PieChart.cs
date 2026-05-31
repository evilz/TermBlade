using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A pie chart component displaying proportional slices with optional animation.
/// </summary>
public sealed class PieChart : RenderableComponentBase<PieChartRenderable>
{
  [Parameter] public IReadOnlyList<PieSlice> Data { get; set; } = [];
  [Parameter] public bool ShowLabels { get; set; } = true;
  [Parameter] public bool ShowPercentages { get; set; } = true;
  [Parameter] public string LabelColor { get; set; } = "#8b949e";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public double AnimationDuration { get; set; }
  [Parameter] public IReadOnlyList<string>? Palette { get; set; }

  protected override PieChartRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(PieChartRenderable renderable)
  {
    renderable.Data = Data;
    renderable.ShowLabels = ShowLabels;
    renderable.ShowPercentages = ShowPercentages;
    renderable.LabelColor = LabelColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.AnimationDuration = AnimationDuration;
    if (Palette != null)
      renderable.Palette = Palette;
  }
}
