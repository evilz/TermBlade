using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Data point for pie and doughnut charts.
/// </summary>
public readonly record struct PieSlice(double Value, string? Label = null, string? Color = null);

/// <summary>
/// A pie chart renderable that draws proportional slices in a circle
/// using Unicode block characters. Supports an optional intro animation
/// that sweeps slices in over time.
/// </summary>
public class PieChartRenderable : Renderable
{
  /// <summary>Slices to display.</summary>
  public IReadOnlyList<PieSlice> Data { get; set; } = [];

  /// <summary>Whether to show percentage labels next to each slice.</summary>
  public bool ShowLabels { get; set; } = true;

  /// <summary>Whether to show percentage values inside/beside slices.</summary>
  public bool ShowPercentages { get; set; } = true;

  /// <summary>CSS color for label text.</summary>
  public string LabelColor { get; set; } = "#8b949e";

  /// <summary>CSS background color.</summary>
  public string? BackgroundColor { get; set; }

  /// <summary>Duration of the intro animation in seconds. 0 disables animation.</summary>
  public double AnimationDuration { get; set; }

  /// <summary>Palette of CSS colors used when slices don't specify their own color.</summary>
  public IReadOnlyList<string> Palette { get; set; } =
    ["#58a6ff", "#3fb950", "#f0883e", "#f85149", "#bc8cff", "#79c0ff", "#d2a8ff", "#56d364"];

  /// <summary>Inner radius ratio (0 = full pie, &gt;0 = doughnut). Subclasses override.</summary>
  protected virtual double InnerRadiusRatio => 0;

  private double _elapsed;
  private bool _animationComplete;

  public PieChartRenderable(CliRenderer? renderer) : base(renderer) { }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0 || Data.Count == 0) return;

    var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);
    var labelFg = Rgba.FromCss(LabelColor);

    double total = 0;
    foreach (var s in Data) total += Math.Max(0, s.Value);
    if (total <= 0) return;

    // Animation progress
    double progress = 1.0;
    if (AnimationDuration > 0)
    {
      _elapsed += deltaTime;
      if (!_animationComplete)
      {
        progress = Math.Clamp(_elapsed / AnimationDuration, 0, 1);
        // Ease-out quad
        progress = 1 - (1 - progress) * (1 - progress);
        if (progress >= 1.0)
          _animationComplete = true;
        else
          RequestRender();
      }
    }

    // Reserve space for legends on the right
    int legendWidth = 0;
    if (ShowLabels)
    {
      foreach (var s in Data)
      {
        int len = (s.Label?.Length ?? 0) + (ShowPercentages ? 8 : 2); // "■ Label 12.3%"
        if (len > legendWidth) legendWidth = len;
      }
      legendWidth += 2; // gap
    }

    int chartW = w - legendWidth;
    if (chartW <= 0) chartW = w;

    // Terminal cells are roughly 2:1 aspect ratio; compensate
    double radiusX = (chartW - 2) / 2.0;
    double radiusY = (h - 2) / 2.0;
    double radius = Math.Min(radiusX, radiusY);
    if (radius < 1) return;

    double cx = x + chartW / 2.0;
    double cy = y + h / 2.0;

    double innerRadius = radius * InnerRadiusRatio;

    // Build slice angles
    var angles = new double[Data.Count + 1];
    angles[0] = -Math.PI / 2; // start at top
    for (int i = 0; i < Data.Count; i++)
    {
      double fraction = Math.Max(0, Data[i].Value) / total;
      angles[i + 1] = angles[i] + fraction * 2 * Math.PI * progress;
    }

    // Resolve colors
    var colors = new Rgba[Data.Count];
    for (int i = 0; i < Data.Count; i++)
    {
      var css = Data[i].Color ?? Palette[i % Palette.Count];
      colors[i] = Rgba.FromCss(css);
    }

    // Render circle cell by cell
    for (int row = 0; row < h; row++)
    {
      for (int col = 0; col < chartW; col++)
      {
        // Map cell to unit circle space (aspect-corrected)
        double px = (col - (cx - x)) / (radius * 2); // terminal chars are ~2:1
        double py = (row - (cy - y)) / radius;
        double dist = Math.Sqrt(px * px + py * py);

        if (dist > 1.0 || dist < InnerRadiusRatio)
          continue;

        double angle = Math.Atan2(py, px);

        // Find which slice this angle falls in
        for (int i = 0; i < Data.Count; i++)
        {
          // Normalize angle comparison
          double a = NormalizeAngle(angle);
          double a0 = NormalizeAngle(angles[i]);
          double a1 = NormalizeAngle(angles[i + 1]);

          bool inSlice;
          if (a0 <= a1)
            inSlice = a >= a0 && a < a1;
          else
            inSlice = a >= a0 || a < a1; // wraps around

          if (inSlice)
          {
            buffer.SetCell(x + col, y + row, '█', colors[i], bg);
            break;
          }
        }
      }
    }

    // Legend
    if (ShowLabels && legendWidth > 0)
    {
      int legendX = x + chartW + 1;
      int legendY = y + Math.Max(0, (h - Data.Count) / 2);
      for (int i = 0; i < Data.Count && legendY + i < y + h; i++)
      {
        double pct = Math.Max(0, Data[i].Value) / total * 100;
        string label = Data[i].Label ?? $"Slice {i + 1}";
        string line = ShowPercentages ? $"■ {label} {pct:F1}%" : $"■ {label}";

        if (legendX + line.Length > x + w)
          line = line[..Math.Max(0, x + w - legendX)];

        if (line.Length > 0)
        {
          // Draw the colored square
          buffer.SetCell(legendX, legendY + i, '■', colors[i], bg);
          // Draw the text
          if (line.Length > 2)
            buffer.DrawText(legendX + 2, legendY + i, line[2..], labelFg, bg);
        }
      }
    }
  }

  private static double NormalizeAngle(double a)
  {
    a %= 2 * Math.PI;
    if (a < 0) a += 2 * Math.PI;
    return a;
  }
}
