using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Provides a reusable single-line label renderable for simple terminal widgets.
/// </summary>
public class LabelRenderable : Renderable
{
  /// <summary>Text content displayed by the widget.</summary>
  public string Content { get; set; } = string.Empty;

  /// <summary>Optional foreground color as a CSS color string.</summary>
  public string? Fg { get; set; }

  /// <summary>Optional background color as a CSS color string.</summary>
  public string? Bg { get; set; }

  /// <summary>Horizontal text alignment: left, center, or right.</summary>
  public string TextAlign { get; set; } = "left";

  /// <summary>Optional text attributes such as bold or underline.</summary>
  public TextAttributes Attributes { get; set; } = TextAttributes.None;

  /// <summary>
  /// Initializes a new instance of the <see cref="LabelRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public LabelRenderable(CliRenderer? renderer) : base(renderer)
  {
  }

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    var w = ComputedWidth;
    var h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = ParseColor(Fg, Rgba.FromInts(255, 255, 255));
    var bg = ParseColor(Bg, Rgba.FromValues(0, 0, 0, 0));
    if (bg.AlphaByte > 0)
      buffer.FillRect(ScreenX, ScreenY, w, h, bg);

    var lines = Content.Split('\n');
    for (var row = 0; row < Math.Min(h, lines.Length); row++)
    {
      var line = TrimToWidth(lines[row], w);
      var x = TextAlign switch
      {
        "center" => ScreenX + Math.Max(0, (w - line.Length) / 2),
        "right" => ScreenX + Math.Max(0, w - line.Length),
        _ => ScreenX,
      };
      buffer.DrawText(x, ScreenY + row, line, fg, bg, Attributes);
    }
  }

  public static Rgba ParseColor(string? color, Rgba fallback)
    => string.IsNullOrWhiteSpace(color) || color == "transparent" ? fallback : Rgba.FromCss(color);

  public static string TrimToWidth(string text, int width)
    => text.Length <= width ? text : text[..Math.Max(0, width)];
}
