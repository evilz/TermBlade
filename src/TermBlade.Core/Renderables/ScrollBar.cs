using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents scroll bar renderable.
/// </summary>
public class ScrollBarRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the orientation.
  /// </summary>
  public string Orientation { get; set; } = "vertical";
  /// <summary>
  /// Gets or sets the scroll position.
  /// </summary>
  public int ScrollPosition { get; set; } = 0;
  /// <summary>
  /// Gets or sets the scroll size.
  /// </summary>
  public int ScrollSize { get; set; } = 100;
  /// <summary>
  /// Gets or sets the viewport size.
  /// </summary>
  public int ViewportSize { get; set; } = 10;
  /// <summary>
  /// Gets or sets the track color.
  /// </summary>
  public string? TrackColor { get; set; } = "#333333";
  /// <summary>
  /// Gets or sets the thumb color.
  /// </summary>
  public string? ThumbColor { get; set; } = "#888888";

  /// <summary>
  /// Scroll bar renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public ScrollBarRenderable(CliRenderer? renderer) : base(renderer) { }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var trackBg = TrackColor != null ? Rgba.FromCss(TrackColor) : Rgba.FromInts(51, 51, 51);
    var thumbFg = ThumbColor != null ? Rgba.FromCss(ThumbColor) : Rgba.FromInts(136, 136, 136);

    bool isVertical = Orientation == "vertical";
    int trackSize = isVertical ? h : w;

    if (ScrollSize <= 0 || ViewportSize >= ScrollSize)
    {
      // Fill all with track
      if (isVertical)
        for (int i = 0; i < h; i++)
          buffer.SetCell(x, y + i, '│', trackBg, trackBg);
      else
        for (int i = 0; i < w; i++)
          buffer.SetCell(x + i, y, '─', trackBg, trackBg);
      return;
    }

    float thumbRatio = (float)ViewportSize / ScrollSize;
    int thumbSize = Math.Max(1, (int)(trackSize * thumbRatio));
    float scrollRatio = (float)ScrollPosition / (ScrollSize - ViewportSize);
    int thumbStart = (int)(scrollRatio * (trackSize - thumbSize));

    for (int i = 0; i < trackSize; i++)
    {
      bool isThumb = i >= thumbStart && i < thumbStart + thumbSize;
      if (isVertical)
        buffer.SetCell(x, y + i, isThumb ? '█' : '│',
            isThumb ? thumbFg : trackBg, trackBg);
      else
        buffer.SetCell(x + i, y, isThumb ? '█' : '─',
            isThumb ? thumbFg : trackBg, trackBg);
    }
  }
}
