using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A candlestick chart component for displaying financial OHLC data.
/// </summary>
public sealed class CandlestickChart : RenderableComponentBase<CandlestickChartRenderable>
{
  [Parameter] public IReadOnlyList<CandlestickPoint> Data { get; set; } = [];
  [Parameter] public string BullColor { get; set; } = "#3fb950";
  [Parameter] public string BearColor { get; set; } = "#f85149";
  [Parameter] public string LabelColor { get; set; } = "#8b949e";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public bool ShowYAxis { get; set; } = true;
  [Parameter] public bool ShowXAxis { get; set; } = true;
  [Parameter] public int CandleWidth { get; set; } = 3;
  [Parameter] public int CandleGap { get; set; } = 1;
  [Parameter] public double? MinValue { get; set; }
  [Parameter] public double? MaxValue { get; set; }

  protected override CandlestickChartRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(CandlestickChartRenderable renderable)
  {
    renderable.Data = Data;
    renderable.BullColor = BullColor;
    renderable.BearColor = BearColor;
    renderable.LabelColor = LabelColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.ShowYAxis = ShowYAxis;
    renderable.ShowXAxis = ShowXAxis;
    renderable.CandleWidth = CandleWidth;
    renderable.CandleGap = CandleGap;
    renderable.MinValue = MinValue;
    renderable.MaxValue = MaxValue;
  }
}
