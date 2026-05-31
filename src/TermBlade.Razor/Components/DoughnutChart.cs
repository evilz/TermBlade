using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A doughnut chart component — a pie chart with a hollow center and optional center label.
/// </summary>
public sealed class DoughnutChart : RenderableComponentBase<DoughnutChartRenderable>
{
  [Parameter] public IReadOnlyList<PieSlice> Data { get; set; } = [];
  [Parameter] public bool ShowLabels { get; set; } = true;
  [Parameter] public bool ShowPercentages { get; set; } = true;
  [Parameter] public string LabelColor { get; set; } = "#8b949e";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public double AnimationDuration { get; set; }
  [Parameter] public IReadOnlyList<string>? Palette { get; set; }
  [Parameter] public double InnerRadius { get; set; } = 0.5;
  [Parameter] public string? CenterLabel { get; set; }
  [Parameter] public string CenterLabelColor { get; set; } = "#c9d1d9";

  protected override DoughnutChartRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(DoughnutChartRenderable renderable)
  {
    renderable.Data = Data;
    renderable.ShowLabels = ShowLabels;
    renderable.ShowPercentages = ShowPercentages;
    renderable.LabelColor = LabelColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.AnimationDuration = AnimationDuration;
    renderable.InnerRadius = InnerRadius;
    renderable.CenterLabel = CenterLabel;
    renderable.CenterLabelColor = CenterLabelColor;
    if (Palette != null)
      renderable.Palette = Palette;
  }
}
