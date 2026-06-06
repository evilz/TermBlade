using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

public readonly record struct SegmentedTextSegment(
    string Text,
    string? Fg = null,
    string? Bg = null,
    TextAttributes Attributes = TextAttributes.None);

public sealed class SegmentedTextRenderable : Renderable
{
  public IReadOnlyList<SegmentedTextSegment> Segments { get; set; } = [];
  public string BackgroundColor { get; set; } = "transparent";

  public SegmentedTextRenderable(CliRenderer? renderer) : base(renderer)
  {
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    var x = ScreenX;
    var y = ScreenY;
    var width = ComputedWidth;
    var height = ComputedHeight;
    if (width <= 0 || height <= 0)
      return;

    var defaultFg = Rgba.FromInts(255, 255, 255);
    var defaultBg = ParseColor(BackgroundColor);
    if (defaultBg.AlphaByte > 0)
      buffer.FillRect(x, y, width, height, defaultBg);

    var col = 0;
    var row = 0;
    foreach (var segment in Segments)
    {
      var fg = segment.Fg == null ? defaultFg : Rgba.FromCss(segment.Fg);
      var bg = segment.Bg == null ? defaultBg : Rgba.FromCss(segment.Bg);

      foreach (var rune in segment.Text.EnumerateRunes())
      {
        if (rune.Value == '\n')
        {
          row++;
          col = 0;
          if (row >= height)
            return;
          continue;
        }

        var runeWidth = CellBuffer.RuneWidth(rune);
        if (col + runeWidth > width)
          continue;

        buffer.SetCell(x + col, y + row, rune.Value, fg, bg, segment.Attributes);
        if (runeWidth == 2 && col + 1 < width)
          buffer.SetCell(x + col + 1, y + row, 0, fg, bg, segment.Attributes);

        col += runeWidth;
      }
    }
  }

  private static Rgba ParseColor(string color)
      => string.IsNullOrEmpty(color) || color == "transparent"
          ? Rgba.FromValues(0, 0, 0, 0)
          : Rgba.FromCss(color);
}
