using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// A line chart renderable that draws data points connected by lines
/// using Braille characters for sub-cell resolution.
/// </summary>
public class LineChartRenderable : Renderable
{
  /// <summary>Data values (Y axis) to plot.</summary>
  public IReadOnlyList<double> Data { get; set; } = [];

  /// <summary>CSS color for the line.</summary>
  public string LineColor { get; set; } = "#58a6ff";

  /// <summary>CSS color for the area fill below the line.</summary>
  public string? FillColor { get; set; }

  /// <summary>CSS color for labels/axis text.</summary>
  public string LabelColor { get; set; } = "#8b949e";

  /// <summary>CSS background color.</summary>
  public string? BackgroundColor { get; set; }

  /// <summary>Whether to show the Y-axis labels.</summary>
  public bool ShowYAxis { get; set; } = true;

  /// <summary>Whether to show the X-axis labels.</summary>
  public bool ShowXAxis { get; set; } = true;

  /// <summary>Optional title displayed at the top-left.</summary>
  public string? Title { get; set; }

  /// <summary>Optional minimum value for Y axis. If null, auto-scales.</summary>
  public double? MinValue { get; set; }

  /// <summary>Optional maximum value for Y axis. If null, auto-scales.</summary>
  public double? MaxValue { get; set; }

  /// <summary>Duration of the intro animation in seconds. 0 disables animation.</summary>
  public double AnimationDuration { get; set; }

  private double _elapsed;
  private bool _animationComplete;

  public LineChartRenderable(CliRenderer? renderer) : base(renderer) { }

  // Braille patterns: each cell is 2x4 dots (2 cols, 4 rows)
  // Dot positions in a Braille character (Unicode U+2800 base):
  // bit0: (0,0)  bit3: (1,0)
  // bit1: (0,1)  bit4: (1,1)
  // bit2: (0,2)  bit5: (1,2)
  // bit6: (0,3)  bit7: (1,3)
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
      foreach (var v in Data)
      {
        if (MinValue == null && v < dataMin) dataMin = v;
        if (MaxValue == null && v > dataMax) dataMax = v;
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
    int yAxisWidth = ShowYAxis ? 7 : 0; // space for Y labels like "25.00"
    int xAxisHeight = ShowXAxis ? 1 : 0;
    int chartX = x + yAxisWidth;
    int chartY = y + titleRows;
    int chartW = w - yAxisWidth;
    int chartH = h - titleRows - xAxisHeight;
    if (chartW <= 0 || chartH <= 0) return;

    // Sub-cell resolution: each cell = 2 dots wide, 4 dots tall
    int dotsW = chartW * 2;
    int dotsH = chartH * 4;

    // Map data points to dot coordinates
    var pointsY = new int[Data.Count];
    for (int i = 0; i < Data.Count; i++)
    {
      double ratio = (Data[i] - dataMin) / (dataMax - dataMin);
      pointsY[i] = (int)((1.0 - ratio) * (dotsH - 1));
    }

    // Create braille grid
    var braille = new int[chartW * chartH];

    // Fill area under line if requested
    Rgba? fillFg = FillColor != null ? Rgba.FromCss(FillColor) : null;

    // Animation: progressive reveal of data points
    int visiblePoints = Data.Count;
    if (AnimationDuration > 0)
    {
      _elapsed += deltaTime;
      if (!_animationComplete)
      {
        double progress = Math.Clamp(_elapsed / AnimationDuration, 0, 1);
        progress = 1 - (1 - progress) * (1 - progress); // ease-out
        visiblePoints = Math.Clamp((int)(Data.Count * progress), 1, Data.Count);
        if (progress >= 1.0)
          _animationComplete = true;
        else
          RequestRender();
      }
    }

    // Draw line segments between data points using Braille
    for (int i = 0; i < visiblePoints - 1; i++)
    {
      double x0 = (double)i / (Data.Count - 1) * (dotsW - 1);
      double x1 = (double)(i + 1) / (Data.Count - 1) * (dotsW - 1);
      int y0 = pointsY[i];
      int y1 = pointsY[i + 1];

      // Bresenham-style line drawing in dot space
      int steps = Math.Max(Math.Abs((int)x1 - (int)x0), Math.Abs(y1 - y0));
      if (steps == 0) steps = 1;

      for (int s = 0; s <= steps; s++)
      {
        int dx = (int)(x0 + (x1 - x0) * s / steps);
        int dy = (int)(y0 + (double)(y1 - y0) * s / steps);
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
          // Check if this cell is below the line for area fill
          // Find the approximate Y position of the line at this column
          double dataX = (double)cx * 2 / (dotsW - 1) * (Data.Count - 1);
          int idx = Math.Clamp((int)dataX, 0, Data.Count - 2);
          double frac = dataX - idx;
          double lineY = pointsY[idx] + (pointsY[idx + 1] - pointsY[idx]) * frac;
          int lineCellY = (int)(lineY / 4);

          if (cy > lineCellY)
          {
            // Gradient fill: fade as we go further from line
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
        double ratio = labelCount == 1 ? 0 : (double)i / (labelCount - 1);
        double val = dataMax - ratio * (dataMax - dataMin);
        string label = FormatValue(val).PadLeft(yAxisWidth - 1);
        int ly = chartY + (int)(ratio * (chartH - 1));
        buffer.DrawText(x, ly, label, labelFg, bg);
      }
    }

    // X-axis labels
    if (ShowXAxis && Data.Count > 1)
    {
      int labelY = chartY + chartH;
      int labelCount = Math.Min(Data.Count, chartW / 6);
      if (labelCount < 2) labelCount = 2;
      for (int i = 0; i < labelCount; i++)
      {
        double ratio = (double)i / (labelCount - 1);
        int dataIdx = (int)(ratio * (Data.Count - 1));
        string label = FormatValue(dataIdx);
        int lx = chartX + (int)(ratio * (chartW - 1));
        lx = Math.Min(lx, x + w - label.Length);
        buffer.DrawText(lx, labelY, label, labelFg, bg);
      }
    }
  }

  private static string FormatValue(double value)
  {
    return Math.Abs(value) >= 1000 ? value.ToString("F0")
         : Math.Abs(value) >= 1 ? value.ToString("F2")
         : value.ToString("F2");
  }
}
