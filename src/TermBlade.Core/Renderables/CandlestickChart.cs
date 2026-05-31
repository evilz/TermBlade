using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// A candlestick chart renderable for displaying financial OHLC data.
/// Each candle shows Open, High, Low, Close values with color-coded bodies.
/// </summary>
public class CandlestickChartRenderable : Renderable
{
  /// <summary>OHLC data points to display.</summary>
  public IReadOnlyList<CandlestickPoint> Data { get; set; } = [];

  /// <summary>CSS color for bullish (close &gt;= open) candles.</summary>
  public string BullColor { get; set; } = "#3fb950";

  /// <summary>CSS color for bearish (close is less than open) candles.</summary>
  public string BearColor { get; set; } = "#f85149";

  /// <summary>CSS color for labels/axis text.</summary>
  public string LabelColor { get; set; } = "#8b949e";

  /// <summary>CSS background color.</summary>
  public string? BackgroundColor { get; set; }

  /// <summary>Whether to show Y-axis labels.</summary>
  public bool ShowYAxis { get; set; } = true;

  /// <summary>Whether to show X-axis labels.</summary>
  public bool ShowXAxis { get; set; } = true;

  /// <summary>Width of each candle in terminal columns (must be odd for centered wick).</summary>
  public int CandleWidth { get; set; } = 3;

  /// <summary>Gap between candles in terminal columns.</summary>
  public int CandleGap { get; set; } = 1;

  /// <summary>Optional minimum Y value. Auto-scales if null.</summary>
  public double? MinValue { get; set; }

  /// <summary>Optional maximum Y value. Auto-scales if null.</summary>
  public double? MaxValue { get; set; }

  /// <summary>Duration of the intro animation in seconds. 0 disables animation.</summary>
  public double AnimationDuration { get; set; }

  private double _elapsed;
  private bool _animationComplete;

  public CandlestickChartRenderable(CliRenderer? renderer) : base(renderer) { }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0 || Data.Count == 0) return;

    var bullFg = Rgba.FromCss(BullColor);
    var bearFg = Rgba.FromCss(BearColor);
    var labelFg = Rgba.FromCss(LabelColor);
    var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);

    // Determine price range
    double priceMin = MinValue ?? double.MaxValue;
    double priceMax = MaxValue ?? double.MinValue;
    if (MinValue == null || MaxValue == null)
    {
      foreach (var c in Data)
      {
        if (MinValue == null)
        {
          if (c.Low < priceMin) priceMin = c.Low;
        }
        if (MaxValue == null)
        {
          if (c.High > priceMax) priceMax = c.High;
        }
      }
    }
    // Add 5% padding
    double range = priceMax - priceMin;
    if (range < 0.0001) range = 1;
    priceMin -= range * 0.02;
    priceMax += range * 0.02;
    range = priceMax - priceMin;

    // Layout
    int yAxisWidth = ShowYAxis ? 8 : 0;
    int xAxisHeight = ShowXAxis ? 1 : 0;
    int chartX = x + yAxisWidth;
    int chartY = y;
    int chartW = w - yAxisWidth;
    int chartH = h - xAxisHeight;
    if (chartW <= 0 || chartH <= 0) return;

    // How many candles fit
    int totalCandleWidth = CandleWidth + CandleGap;
    int maxVisibleCandles = Math.Min(Data.Count, chartW / Math.Max(1, totalCandleWidth));

    // Animation: progressive reveal of candles
    int visibleCandles = maxVisibleCandles;
    if (AnimationDuration > 0)
    {
      _elapsed += deltaTime;
      if (!_animationComplete)
      {
        double progress = Math.Clamp(_elapsed / AnimationDuration, 0, 1);
        progress = 1 - (1 - progress) * (1 - progress); // ease-out
        visibleCandles = Math.Max(1, (int)(maxVisibleCandles * progress));
        if (progress >= 1.0)
          _animationComplete = true;
        else
          RequestRender();
      }
    }

    int startIdx = Math.Max(0, Data.Count - visibleCandles);

    for (int i = 0; i < visibleCandles; i++)
    {
      var candle = Data[startIdx + i];
      bool isBull = candle.Close >= candle.Open;
      var candleFg = isBull ? bullFg : bearFg;

      int cx = chartX + i * totalCandleWidth;
      int wickX = cx + CandleWidth / 2;

      // Map prices to Y coordinates
      int highY = chartY + (int)((1.0 - (candle.High - priceMin) / range) * (chartH - 1));
      int lowY = chartY + (int)((1.0 - (candle.Low - priceMin) / range) * (chartH - 1));
      int openY = chartY + (int)((1.0 - (candle.Open - priceMin) / range) * (chartH - 1));
      int closeY = chartY + (int)((1.0 - (candle.Close - priceMin) / range) * (chartH - 1));

      highY = Math.Clamp(highY, chartY, chartY + chartH - 1);
      lowY = Math.Clamp(lowY, chartY, chartY + chartH - 1);
      openY = Math.Clamp(openY, chartY, chartY + chartH - 1);
      closeY = Math.Clamp(closeY, chartY, chartY + chartH - 1);

      int bodyTop = Math.Min(openY, closeY);
      int bodyBottom = Math.Max(openY, closeY);

      // Draw upper wick
      for (int wy = highY; wy < bodyTop; wy++)
      {
        if (wickX < x + w)
          buffer.SetCell(wickX, wy, '│', candleFg, bg);
      }

      // Draw body
      if (bodyTop == bodyBottom)
      {
        // Doji - single line
        for (int bx = 0; bx < CandleWidth && cx + bx < x + w; bx++)
          buffer.SetCell(cx + bx, bodyTop, '─', candleFg, bg);
      }
      else
      {
        for (int by = bodyTop; by <= bodyBottom; by++)
        {
          char ch = isBull ? '█' : '█';
          for (int bx = 0; bx < CandleWidth && cx + bx < x + w; bx++)
            buffer.SetCell(cx + bx, by, ch, candleFg, bg);
        }
      }

      // Draw lower wick
      for (int wy = bodyBottom + 1; wy <= lowY; wy++)
      {
        if (wickX < x + w)
          buffer.SetCell(wickX, wy, '│', candleFg, bg);
      }
    }

    // Y-axis labels
    if (ShowYAxis)
    {
      int labelCount = Math.Min(chartH, 8);
      for (int i = 0; i < labelCount; i++)
      {
        double ratio = (double)i / (labelCount - 1);
        double val = priceMax - ratio * range;
        string label = FormatPrice(val).PadLeft(yAxisWidth - 1);
        int ly = chartY + (int)(ratio * (chartH - 1));
        buffer.DrawText(x, ly, label, labelFg, bg);
      }
    }

    // X-axis labels
    if (ShowXAxis && visibleCandles > 0)
    {
      int labelY = chartY + chartH;
      int maxLabels = chartW / 10;
      int labelCount = Math.Clamp(maxLabels, 2, visibleCandles);
      for (int i = 0; i < labelCount; i++)
      {
        double ratio = (double)i / (labelCount - 1);
        int dataIdx = startIdx + (int)(ratio * (visibleCandles - 1));
        dataIdx = Math.Clamp(dataIdx, 0, Data.Count - 1);
        var candle = Data[dataIdx];
        string label = candle.Label ?? (dataIdx + 1).ToString();
        int lx = chartX + (int)(ratio * (visibleCandles - 1)) * totalCandleWidth;
        lx = Math.Min(lx, x + w - label.Length);
        buffer.DrawText(lx, labelY, label, labelFg, bg);
      }
    }
  }

  private static string FormatPrice(double value)
  {
    return Math.Abs(value) >= 1000 ? value.ToString("F0")
         : value.ToString("F2");
  }
}

/// <summary>
/// Represents a single candlestick (OHLC) data point.
/// </summary>
public readonly record struct CandlestickPoint(
    double Open,
    double High,
    double Low,
    double Close,
    string? Label = null);
