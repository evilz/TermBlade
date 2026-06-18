using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// A doughnut chart renderable — a pie chart with a hollow center.
/// Inherits all rendering logic from <see cref="PieChartRenderable"/> with a non-zero inner radius.
/// </summary>
public class DoughnutChartRenderable : PieChartRenderable
{
  /// <summary>Inner radius as a fraction of the outer radius (0–1). Defaults to 0.5.</summary>
  public double InnerRadius { get; set; } = 0.5;

  protected override double InnerRadiusRatio => Math.Clamp(InnerRadius, 0, 0.95);

  /// <summary>Optional center label displayed in the doughnut hole.</summary>
  public string? CenterLabel { get; set; }

  /// <summary>CSS color for the center label text.</summary>
  public string CenterLabelColor { get; set; } = "#c9d1d9";

  /// <summary>
  /// Doughnut chart renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public DoughnutChartRenderable(CliRenderer? renderer) : base(renderer) { }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    base.RenderSelf(buffer, deltaTime);

    // Draw center label if specified
    if (CenterLabel == null) return;

    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    // Estimate chart area (same logic as base)
    int legendWidth = 0;
    if (ShowLabels)
      legendWidth = GetLegendWidth();
    int chartW = w - legendWidth;
    if (chartW <= 0) chartW = w;

    int centerX = x + chartW / 2;
    int centerY = y + h / 2;
    int labelStart = centerX - CenterLabel.Length / 2;
    labelStart = Math.Max(x, labelStart);

    var fg = Rgba.FromCss(CenterLabelColor);
    var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);

    string text = CenterLabel;
    if (labelStart + text.Length > x + chartW)
      text = text[..Math.Max(0, x + chartW - labelStart)];

    if (text.Length > 0)
      buffer.DrawText(labelStart, centerY, text, fg, bg);
  }
}
