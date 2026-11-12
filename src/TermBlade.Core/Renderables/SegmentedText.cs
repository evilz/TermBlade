using System.Globalization;
using System.Text;
using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents struct.
/// </summary>
public readonly record struct SegmentedTextSegment(
    string Text,
    string? Fg = null,
    string? Bg = null,
    TextAttributes Attributes = TextAttributes.None);

/// <summary>
/// Represents segmented text renderable.
/// </summary>
public sealed class SegmentedTextRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the segments.
  /// </summary>
  public IReadOnlyList<SegmentedTextSegment> Segments { get; set; } = [];
  /// <summary>
  /// Gets or sets the background color.
  /// </summary>
  public string BackgroundColor { get; set; } = "transparent";

  /// <summary>
  /// Segmented text renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
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

    var text = new StringBuilder();
    var styles = new List<(int End, Rgba Fg, Rgba Bg, TextAttributes Attributes)>();
    foreach (var segment in Segments)
    {
      var fg = segment.Fg == null ? defaultFg : Rgba.FromCss(segment.Fg);
      var bg = segment.Bg == null ? defaultBg : Rgba.FromCss(segment.Bg);
      text.Append(segment.Text);
      styles.Add((text.Length, fg, bg, segment.Attributes));
    }

    var fullText = text.ToString();
    int col = 0;
    int row = 0;
    int styleIndex = 0;
    var remaining = fullText.AsSpan();
    while (!remaining.IsEmpty)
    {
      int graphemeLength = StringInfo.GetNextTextElementLength(remaining);
      var grapheme = remaining[..graphemeLength];
      while (styleIndex + 1 < styles.Count && fullText.Length - remaining.Length >= styles[styleIndex].End)
        styleIndex++;

      if (grapheme[0] is '\r' or '\n')
      {
        row++;
        col = 0;
        if (row >= height)
          return;
        remaining = remaining[graphemeLength..];
        continue;
      }

      int cellWidth = CellBuffer.GraphemeWidth(grapheme);
      if (cellWidth > 0)
      {
        if (col + cellWidth > width)
          break;

        var style = styles[styleIndex];
        buffer.DrawGrapheme(x + col, y + row, grapheme, cellWidth,
            style.Fg, style.Bg, style.Attributes);
        col += cellWidth;
      }

      remaining = remaining[graphemeLength..];
    }
  }

  private static Rgba ParseColor(string color)
      => string.IsNullOrEmpty(color) || color == "transparent"
          ? Rgba.FromValues(0, 0, 0, 0)
          : Rgba.FromCss(color);
}
