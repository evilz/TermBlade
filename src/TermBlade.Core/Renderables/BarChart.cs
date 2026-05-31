using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// A bar chart renderable supporting vertical and horizontal orientations,
/// with optional sparkline mode for compact single-row/column display.
/// </summary>
public class BarChartRenderable : Renderable
{
  /// <summary>Data values to display as bars.</summary>
  public IReadOnlyList<double> Data { get; set; } = [];

  /// <summary>Optional labels for each bar.</summary>
  public IReadOnlyList<string>? Labels { get; set; }

  /// <summary>"vertical" or "horizontal".</summary>
  public string Orientation { get; set; } = "vertical";

  /// <summary>When true, renders as a single-row sparkline using block characters.</summary>
  public bool Sparkline { get; set; }

  /// <summary>CSS color for the bars. Defaults to a bright cyan.</summary>
  public string BarColor { get; set; } = "#58a6ff";

  /// <summary>CSS color for labels and axis text.</summary>
  public string LabelColor { get; set; } = "#8b949e";

  /// <summary>CSS background color.</summary>
  public string? BackgroundColor { get; set; }

  /// <summary>Gap between bars in non-sparkline mode (in cells).</summary>
  public int BarGap { get; set; } = 1;

  /// <summary>Width of each bar in non-sparkline mode (in cells).</summary>
  public int BarWidth { get; set; } = 3;

  /// <summary>Optional maximum value for the axis. If null, auto-scales to data max.</summary>
  public double? MaxValue { get; set; }

  public BarChartRenderable(CliRenderer? renderer) : base(renderer) { }

  // Block characters for fractional bar rendering (eighths from empty to full)
  private static readonly char[] VerticalBlocks = [' ', '▁', '▂', '▃', '▄', '▅', '▆', '▇', '█'];
  private static readonly char[] HorizontalBlocks = [' ', '▏', '▎', '▍', '▌', '▋', '▊', '▉', '█'];

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0 || Data.Count == 0) return;

    var barFg = Rgba.FromCss(BarColor);
    var labelFg = Rgba.FromCss(LabelColor);
    var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);

    double max = MaxValue ?? 0;
    foreach (var v in Data)
      if (v > max) max = v;
    if (max <= 0) max = 1;

    if (Sparkline)
    {
      RenderSparkline(buffer, x, y, w, h, barFg, bg, max);
    }
    else if (Orientation == "horizontal")
    {
      RenderHorizontal(buffer, x, y, w, h, barFg, labelFg, bg, max);
    }
    else
    {
      RenderVertical(buffer, x, y, w, h, barFg, labelFg, bg, max);
    }
  }

  private void RenderSparkline(RenderBuffer buffer, int x, int y, int w, int h, Rgba fg, Rgba bg, double max)
  {
    bool isVertical = Orientation != "horizontal";

    if (isVertical)
    {
      // Each data point gets one column, bar height determined by data
      int count = Math.Min(Data.Count, w);
      for (int i = 0; i < count; i++)
      {
        double ratio = Data[i] / max;
        double fullHeight = ratio * h;
        int fullBlocks = (int)fullHeight;
        int fractional = (int)((fullHeight - fullBlocks) * 8);

        for (int row = 0; row < h; row++)
        {
          int barRow = h - 1 - row;
          char ch;
          if (barRow < fullBlocks)
            ch = '█';
          else if (barRow == fullBlocks && fractional > 0)
            ch = VerticalBlocks[fractional];
          else
            ch = ' ';

          buffer.SetCell(x + i, y + row, ch, fg, bg);
        }
      }
    }
    else
    {
      // Each data point gets one row, bar width determined by data
      int count = Math.Min(Data.Count, h);
      for (int i = 0; i < count; i++)
      {
        double ratio = Data[i] / max;
        double fullWidth = ratio * w;
        int fullBlocks = (int)fullWidth;
        int fractional = (int)((fullWidth - fullBlocks) * 8);

        for (int col = 0; col < w; col++)
        {
          char ch;
          if (col < fullBlocks)
            ch = '█';
          else if (col == fullBlocks && fractional > 0)
            ch = HorizontalBlocks[fractional];
          else
            ch = ' ';

          buffer.SetCell(x + col, y + i, ch, fg, bg);
        }
      }
    }
  }

  private void RenderVertical(RenderBuffer buffer, int x, int y, int w, int h, Rgba barFg, Rgba labelFg, Rgba bg, double max)
  {
    // Reserve 1 row at bottom for labels if available
    bool hasLabels = Labels != null && Labels.Count > 0;
    int chartHeight = hasLabels ? h - 1 : h;
    if (chartHeight <= 0) return;

    int totalBarWidth = BarWidth + BarGap;
    int count = Math.Min(Data.Count, w / Math.Max(1, totalBarWidth));

    for (int i = 0; i < count; i++)
    {
      int bx = x + i * totalBarWidth;
      double ratio = Data[i] / max;
      double fullHeight = ratio * chartHeight;
      int fullBlocks = (int)fullHeight;
      int fractional = (int)((fullHeight - fullBlocks) * 8);

      for (int row = 0; row < chartHeight; row++)
      {
        int barRow = chartHeight - 1 - row;
        char ch;
        if (barRow < fullBlocks)
          ch = '█';
        else if (barRow == fullBlocks && fractional > 0)
          ch = VerticalBlocks[fractional];
        else
          ch = ' ';

        for (int bw = 0; bw < BarWidth && bx + bw < x + w; bw++)
          buffer.SetCell(bx + bw, y + row, ch, barFg, bg);
      }

      // Label
      if (hasLabels && i < Labels!.Count)
      {
        var label = Labels[i];
        if (label.Length > BarWidth) label = label[..BarWidth];
        buffer.DrawText(bx, y + chartHeight, label, labelFg, bg);
      }
    }
  }

  private void RenderHorizontal(RenderBuffer buffer, int x, int y, int w, int h, Rgba barFg, Rgba labelFg, Rgba bg, double max)
  {
    // Reserve space for labels on the left
    int labelWidth = 0;
    if (Labels != null)
    {
      foreach (var l in Labels)
        if (l.Length > labelWidth) labelWidth = l.Length;
      labelWidth += 1; // gap
    }

    int chartWidth = w - labelWidth;
    if (chartWidth <= 0) return;

    int totalBarHeight = 1 + BarGap;
    int count = Math.Min(Data.Count, h / Math.Max(1, totalBarHeight));

    for (int i = 0; i < count; i++)
    {
      int by = y + i * totalBarHeight;
      double ratio = Data[i] / max;
      double fullWidth = ratio * chartWidth;
      int fullBlocks = (int)fullWidth;
      int fractional = (int)((fullWidth - fullBlocks) * 8);

      // Label
      if (Labels != null && i < Labels.Count)
      {
        var label = Labels[i].PadLeft(labelWidth - 1);
        buffer.DrawText(x, by, label, labelFg, bg);
      }

      // Bar
      for (int col = 0; col < chartWidth; col++)
      {
        char ch;
        if (col < fullBlocks)
          ch = '█';
        else if (col == fullBlocks && fractional > 0)
          ch = HorizontalBlocks[fractional];
        else
          ch = ' ';

        buffer.SetCell(x + labelWidth + col, by, ch, barFg, bg);
      }
    }
  }
}
