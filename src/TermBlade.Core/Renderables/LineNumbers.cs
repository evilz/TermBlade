using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents line numbers renderable.
/// </summary>
public class LineNumbersRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the line count.
  /// </summary>
  public int LineCount { get; set; } = 0;
  /// <summary>
  /// Gets or sets the start line.
  /// </summary>
  public int StartLine { get; set; } = 1;
  /// <summary>
  /// Gets or sets the fg.
  /// </summary>
  public string? Fg { get; set; } = "#666666";
  /// <summary>
  /// Gets or sets the bg.
  /// </summary>
  public string? Bg { get; set; }

  /// <summary>
  /// Line numbers renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public LineNumbersRenderable(CliRenderer? renderer) : base(renderer) { }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(100, 100, 100);
    var bg = Bg != null ? Rgba.FromCss(Bg) : Rgba.FromInts(0, 0, 0, 0);

    for (int row = 0; row < h && row < LineCount; row++)
    {
      var num = (StartLine + row).ToString().PadLeft(w);
      if (num.Length > w) num = num[..w];
      buffer.DrawText(x, y + row, num, fg, bg);
    }
  }
}
