using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A heat map component that displays a 2D grid of values as colored cells.
/// </summary>
public sealed class HeatMap : RenderableComponentBase<HeatMapRenderable>
{
  [Parameter] public IReadOnlyList<IReadOnlyList<double>> Data { get; set; } = [];
  [Parameter] public IReadOnlyList<string>? ColumnLabels { get; set; }
  [Parameter] public IReadOnlyList<string>? RowLabels { get; set; }
  [Parameter] public string LowColor { get; set; } = "#0d1117";
  [Parameter] public string HighColor { get; set; } = "#58a6ff";
  [Parameter] public string? MidColor { get; set; }
  [Parameter] public string LabelColor { get; set; } = "#8b949e";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public bool ShowValues { get; set; }
  [Parameter] public int CellWidth { get; set; } = 4;
  [Parameter] public int CellHeight { get; set; } = 1;
  [Parameter] public double? MinValue { get; set; }
  [Parameter] public double? MaxValue { get; set; }
  [Parameter] public double AnimationDuration { get; set; }

  protected override HeatMapRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(HeatMapRenderable renderable)
  {
    renderable.Data = Data;
    renderable.ColumnLabels = ColumnLabels;
    renderable.RowLabels = RowLabels;
    renderable.LowColor = LowColor;
    renderable.HighColor = HighColor;
    renderable.MidColor = MidColor;
    renderable.LabelColor = LabelColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.ShowValues = ShowValues;
    renderable.CellWidth = CellWidth;
    renderable.CellHeight = CellHeight;
    renderable.MinValue = MinValue;
    renderable.MaxValue = MaxValue;
    renderable.AnimationDuration = AnimationDuration;
  }
}
