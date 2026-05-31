using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// A time-series line chart that plots timestamped data points.
/// Supports auto-scrolling and time-based X-axis labels.
/// </summary>
public class TimeSeriesLineChartRenderable : Renderable
{
  /// <summary>Time-series data points as (timestamp, value) pairs.</summary>
  public IReadOnlyList<TimeSeriesPoint> Data { get; set; } = [];

  /// <summary>CSS color for the line.</summary>
  public string LineColor { get; set; } = "#58a6ff";

  /// <summary>CSS color for the area fill below the line.</summary>
  public string? FillColor { get; set; }

  /// <summary>CSS color for labels/axis text.</summary>
  public string LabelColor { get; set; } = "#8b949e";

  /// <summary>CSS background color.</summary>
  public string? BackgroundColor { get; set; }

  /// <summary>Whether to show Y-axis labels.</summary>
  public bool ShowYAxis { get; set; } = true;

  /// <summary>Whether to show X-axis time labels.</summary>
  public bool ShowXAxis { get; set; } = true;

  /// <summary>Optional title displayed at the top-left.</summary>
  public string? Title { get; set; }

  /// <summary>Time format for X-axis labels. Defaults to "HH:mm".</summary>
  public string TimeFormat { get; set; } = "HH:mm";

  /// <summary>Optional minimum Y value. Auto-scales if null.</summary>
  public double? MinValue { get; set; }

  /// <summary>Optional maximum Y value. Auto-scales if null.</summary>
  public double? MaxValue { get; set; }

  /// <summary>Duration of the intro animation in seconds. 0 disables animation.</summary>
  public double AnimationDuration { get; set; }

  private double _elapsed;
  private bool _animationComplete;

  public TimeSeriesLineChartRenderable(CliRenderer? renderer) : base(renderer) { }

  private static readonly int[] BrailleDotBits = [0x01, 0x02, 0x04, 0x40, 0x08, 0x10, 0x20, 0x80];
  private const int BrailleBase = 0x2800;

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0 || Data.Count == 0) return;

    var lineFg = Rgba.FromCss(LineColor);
    var labelFg = Rgba.FromCss(LabelColor);
    var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);

    // Determine data range
    double dataMin = MinValue ?? double.MaxValue;
    double dataMax = MaxValue ?? double.MinValue;
    if (MinValue == null || MaxValue == null)
    {
      foreach (var p in Data)
      {
        if (MinValue == null && p.Value < dataMin) dataMin = p.Value;
        if (MaxValue == null && p.Value > dataMax) dataMax = p.Value;
      }
    }
    if (Math.Abs(dataMax - dataMin) < 0.0001) dataMax = dataMin + 1;

    // Title row
    int titleRows = 0;
    if (Title != null)
    {
      buffer.DrawText(x, y, Title, lineFg, bg, TextAttributes.Bold);
      titleRows = 1;
    }

    // Layout
    int yAxisWidth = ShowYAxis ? 7 : 0;
    int xAxisHeight = ShowXAxis ? 1 : 0;
    int chartX = x + yAxisWidth;
    int chartY = y + titleRows;
    int chartW = w - yAxisWidth;
    int chartH = h - titleRows - xAxisHeight;
    if (chartW <= 0 || chartH <= 0) return;

    int dotsW = chartW * 2;
    int dotsH = chartH * 4;

    // Map data points to dot coordinates
    DateTimeOffset timeMin = Data[0].Time;
    DateTimeOffset timeMax = Data[^1].Time;
    double timeRange = (timeMax - timeMin).TotalSeconds;
    if (timeRange <= 0) timeRange = 1;

    var pointsX = new int[Data.Count];
    var pointsY = new int[Data.Count];
    for (int i = 0; i < Data.Count; i++)
    {
      double tx = (Data[i].Time - timeMin).TotalSeconds / timeRange;
      double ty = (Data[i].Value - dataMin) / (dataMax - dataMin);
      pointsX[i] = Math.Clamp((int)(tx * (dotsW - 1)), 0, dotsW - 1);
      pointsY[i] = Math.Clamp((int)((1.0 - ty) * (dotsH - 1)), 0, dotsH - 1);
    }

    // Create braille grid
    var braille = new int[chartW * chartH];

    // Animation: progressive reveal
    int visiblePoints = Data.Count;
    if (AnimationDuration > 0)
    {
      _elapsed += deltaTime;
      if (!_animationComplete)
      {
        double progress = Math.Clamp(_elapsed / AnimationDuration, 0, 1);
        progress = 1 - (1 - progress) * (1 - progress);
        visiblePoints = Math.Max(2, (int)(Data.Count * progress));
        if (progress >= 1.0)
          _animationComplete = true;
        else
          RequestRender();
      }
    }

    // Draw line segments
    for (int i = 0; i < visiblePoints - 1; i++)
    {
      int x0 = pointsX[i], x1 = pointsX[i + 1];
      int y0 = pointsY[i], y1 = pointsY[i + 1];
      int steps = Math.Max(Math.Abs(x1 - x0), Math.Abs(y1 - y0));
      if (steps == 0) steps = 1;

      for (int s = 0; s <= steps; s++)
      {
        int dx = x0 + (x1 - x0) * s / steps;
        int dy = y0 + (y1 - y0) * s / steps;
        dx = Math.Clamp(dx, 0, dotsW - 1);
        dy = Math.Clamp(dy, 0, dotsH - 1);

        int cellX = dx / 2;
        int cellY = dy / 4;
        int dotCol = dx % 2;
        int dotRow = dy % 4;

        if (cellX < chartW && cellY < chartH)
          braille[cellY * chartW + cellX] |= BrailleDotBits[dotCol * 4 + dotRow];
      }
    }

    // Fill below line
    Rgba? fillFg = FillColor != null ? Rgba.FromCss(FillColor) : null;

    // Render braille characters
    for (int cy = 0; cy < chartH; cy++)
    {
      for (int cx = 0; cx < chartW; cx++)
      {
        int pattern = braille[cy * chartW + cx];
        if (pattern != 0)
        {
          buffer.SetCell(chartX + cx, chartY + cy, BrailleBase + pattern, lineFg, bg);
        }
        else if (fillFg.HasValue && Data.Count > 1)
        {
          double dataX = (double)cx * 2 / (dotsW - 1) * (Data.Count - 1);
          int idx = Math.Clamp((int)dataX, 0, Data.Count - 2);
          double frac = dataX - idx;
          double lineY = pointsY[idx] + (pointsY[idx + 1] - pointsY[idx]) * frac;
          int lineCellY = (int)(lineY / 4);

          if (cy > lineCellY)
          {
            double dist = (double)(cy - lineCellY) / chartH;
            int alpha = Math.Clamp((int)(255 * (1.0 - dist * 1.5)), 20, 200);
            var fill = Rgba.FromInts(fillFg.Value.RedByte, fillFg.Value.GreenByte, fillFg.Value.BlueByte, alpha);
            buffer.SetCell(chartX + cx, chartY + cy, '░', fill, bg);
          }
        }
      }
    }

    // Y-axis labels
    if (ShowYAxis)
    {
      int labelCount = Math.Min(chartH, 6);
      for (int i = 0; i < labelCount; i++)
      {
        double ratio = (double)i / (labelCount - 1);
        double val = dataMax - ratio * (dataMax - dataMin);
        string label = FormatValue(val).PadLeft(yAxisWidth - 1);
        int ly = chartY + (int)(ratio * (chartH - 1));
        buffer.DrawText(x, ly, label, labelFg, bg);
      }
    }

    // X-axis time labels
    if (ShowXAxis)
    {
      int labelY = chartY + chartH;
      int maxLabels = chartW / 8;
      int labelCount = Math.Clamp(maxLabels, 2, Math.Min(Data.Count, 8));
      for (int i = 0; i < labelCount; i++)
      {
        double ratio = (double)i / (labelCount - 1);
        int dataIdx = Math.Clamp((int)(ratio * (Data.Count - 1)), 0, Data.Count - 1);
        string label = Data[dataIdx].Time.ToString(TimeFormat);
        int lx = chartX + (int)(ratio * (chartW - 1));
        lx = Math.Min(lx, x + w - label.Length);
        buffer.DrawText(lx, labelY, label, labelFg, bg);
      }
    }
  }

  private static string FormatValue(double value)
  {
    return Math.Abs(value) >= 1000 ? value.ToString("F0")
         : value.ToString("F2");
  }
}

/// <summary>
/// Represents a single data point in a time series.
/// </summary>
public readonly record struct TimeSeriesPoint(DateTimeOffset Time, double Value);
