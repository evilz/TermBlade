using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// A heat map renderable that displays a 2D grid of values as colored cells.
/// Values are mapped to colors using a configurable color gradient.
/// </summary>
public class HeatMapRenderable : Renderable
{
  /// <summary>2D data array [rows][columns]. Each value is mapped to a color.</summary>
  public IReadOnlyList<IReadOnlyList<double>> Data { get; set; } = [];

  /// <summary>Optional column labels displayed at the top.</summary>
  public IReadOnlyList<string>? ColumnLabels { get; set; }

  /// <summary>Optional row labels displayed on the left.</summary>
  public IReadOnlyList<string>? RowLabels { get; set; }

  /// <summary>Color for low values. CSS color string.</summary>
  public string LowColor { get; set; } = "#0d1117";

  /// <summary>Color for high values. CSS color string.</summary>
  public string HighColor { get; set; } = "#58a6ff";

  /// <summary>Optional mid-point color for three-stop gradient.</summary>
  public string? MidColor { get; set; }

  /// <summary>CSS color for labels.</summary>
  public string LabelColor { get; set; } = "#8b949e";

  /// <summary>CSS background color.</summary>
  public string? BackgroundColor { get; set; }

  /// <summary>Whether to show values inside cells.</summary>
  public bool ShowValues { get; set; }

  /// <summary>Width of each heat map cell in terminal columns.</summary>
  public int CellWidth { get; set; } = 4;

  /// <summary>Height of each heat map cell in terminal rows.</summary>
  public int CellHeight { get; set; } = 1;

  /// <summary>Optional minimum value. Auto-scales if null.</summary>
  public double? MinValue { get; set; }

  /// <summary>Optional maximum value. Auto-scales if null.</summary>
  public double? MaxValue { get; set; }

  /// <summary>Duration of the intro animation in seconds. 0 disables animation.</summary>
  public double AnimationDuration { get; set; }

  private double _elapsed;
  private bool _animationComplete;

  /// <summary>
  /// Heat map renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public HeatMapRenderable(CliRenderer? renderer) : base(renderer) { }

  // Block characters for intensity: ░ ▒ ▓ █
  private static readonly char[] IntensityChars = ['░', '▒', '▓', '█'];

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0 || Data.Count == 0) return;

    var labelFg = Rgba.FromCss(LabelColor);
    var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);

    // Parse gradient colors
    var lowRgba = Rgba.FromCss(LowColor);
    var highRgba = Rgba.FromCss(HighColor);
    Rgba? midRgba = MidColor != null ? Rgba.FromCss(MidColor) : null;

    // Determine data range
    double dataMin = MinValue ?? double.MaxValue;
    double dataMax = MaxValue ?? double.MinValue;
    if (MinValue == null || MaxValue == null)
    {
      foreach (var row in Data)
        foreach (var v in row)
        {
          if (MinValue == null && v < dataMin) dataMin = v;
          if (MaxValue == null && v > dataMax) dataMax = v;
        }
    }
    if (Math.Abs(dataMax - dataMin) < 0.0001) dataMax = dataMin + 1;

    // Layout: row labels on left
    int rowLabelWidth = 0;
    if (RowLabels != null)
    {
      rowLabelWidth = RowLabels.Select(l => l.Length).DefaultIfEmpty(0).Max();
      rowLabelWidth += 1;
    }

    int colLabelHeight = ColumnLabels != null ? 1 : 0;
    int gridX = x + rowLabelWidth;
    int gridY = y + colLabelHeight;

    int rows = Data.Count;
    int cols = Data.Count > 0 ? Data[0].Count : 0;

    // Column labels
    if (ColumnLabels != null)
    {
      for (int c = 0; c < cols && c < ColumnLabels.Count; c++)
      {
        int cx = gridX + c * CellWidth;
        var label = ColumnLabels[c];
        if (label.Length > CellWidth) label = label[..CellWidth];
        if (cx + label.Length <= x + w)
          buffer.DrawText(cx, y, label, labelFg, bg);
      }
    }

    // Animation: fade-in by scaling ratio toward zero
    double animScale = 1.0;
    if (AnimationDuration > 0)
    {
      _elapsed += deltaTime;
      if (!_animationComplete)
      {
        animScale = Math.Clamp(_elapsed / AnimationDuration, 0, 1);
        animScale = 1 - (1 - animScale) * (1 - animScale); // ease-out
        if (animScale >= 1.0)
          _animationComplete = true;
        else
          RequestRender();
      }
    }

    // Render cells
    for (int r = 0; r < rows; r++)
    {
      int ry = gridY + r * CellHeight;
      if (ry >= y + h) break;

      // Row label
      if (RowLabels != null && r < RowLabels.Count)
      {
        var label = RowLabels[r].PadLeft(rowLabelWidth - 1);
        buffer.DrawText(x, ry, label, labelFg, bg);
      }

      for (int c = 0; c < Data[r].Count; c++)
      {
        int cx = gridX + c * CellWidth;
        if (cx >= x + w) break;

        double ratio = (Data[r][c] - dataMin) / (dataMax - dataMin);
        ratio = Math.Clamp(ratio * animScale, 0, 1);

        var cellColor = InterpolateColor(lowRgba, midRgba, highRgba, ratio);

        // Fill cell
        for (int dy = 0; dy < CellHeight && ry + dy < y + h; dy++)
        {
          for (int dx = 0; dx < CellWidth && cx + dx < x + w; dx++)
          {
            buffer.SetCell(cx + dx, ry + dy, '█', cellColor, bg);
          }
        }

        // Show value text
        if (ShowValues && CellWidth >= 3)
        {
          string val = Data[r][c].ToString("F1");
          if (val.Length > CellWidth) val = val[..CellWidth];
          // Use contrasting text color
          var textFg = GetContrastColor(cellColor);
          buffer.DrawText(cx, ry, val, textFg, cellColor);
        }
      }
    }
  }

  private static Rgba InterpolateColor(Rgba low, Rgba? mid, Rgba high, double ratio)
  {
    if (mid.HasValue)
    {
      if (ratio < 0.5)
      {
        double t = ratio * 2;
        return LerpColor(low, mid.Value, t);
      }
      else
      {
        double t = (ratio - 0.5) * 2;
        return LerpColor(mid.Value, high, t);
      }
    }
    return LerpColor(low, high, ratio);
  }

  private static Rgba LerpColor(Rgba a, Rgba b, double t)
  {
    int r = (int)(a.RedByte + (b.RedByte - a.RedByte) * t);
    int g = (int)(a.GreenByte + (b.GreenByte - a.GreenByte) * t);
    int bl = (int)(a.BlueByte + (b.BlueByte - a.BlueByte) * t);
    return Rgba.FromInts(Math.Clamp(r, 0, 255), Math.Clamp(g, 0, 255), Math.Clamp(bl, 0, 255));
  }

  private static Rgba GetContrastColor(Rgba color)
  {
    // Luminance-based contrast
    double lum = 0.299 * color.RedByte + 0.587 * color.GreenByte + 0.114 * color.BlueByte;
    return lum > 128 ? Rgba.FromInts(0, 0, 0) : Rgba.FromInts(255, 255, 255);
  }
}
