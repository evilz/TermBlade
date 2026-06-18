using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents frame buffer renderable.
/// </summary>
public class FrameBufferRenderable : Renderable
{
  /// <summary>
  /// Gets the buffer width.
  /// </summary>
  public int BufferWidth { get; }
  /// <summary>
  /// Gets the buffer height.
  /// </summary>
  public int BufferHeight { get; }

  private readonly Rgba[] _pixels;

  /// <summary>
  /// Frame buffer renderable.
  /// </summary>
  /// <param name="renderer">The renderer value.</param>
  /// <param name="pixelWidth">The pixelWidth value.</param>
  /// <param name="base(renderer">The base(renderer value.</param>
  public FrameBufferRenderable(CliRenderer? renderer, int pixelWidth, int pixelHeight) : base(renderer)
  {
    BufferWidth = pixelWidth;
    BufferHeight = pixelHeight;
    _pixels = new Rgba[pixelWidth * pixelHeight];
    Clear(Rgba.FromInts(0, 0, 0));
  }

  /// <summary>
  /// Set pixel.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  /// <param name="color">The color value.</param>
  public void SetPixel(int x, int y, Rgba color)
  {
    if (x < 0 || x >= BufferWidth || y < 0 || y >= BufferHeight) return;
    _pixels[y * BufferWidth + x] = color;
  }

  /// <summary>
  /// Clear.
  /// </summary>
  /// <param name="color">The color value.</param>
  public void Clear(Rgba color)
  {
    for (int i = 0; i < _pixels.Length; i++) _pixels[i] = color;
  }

  /// <summary>
  /// Draw line.
  /// </summary>
  /// <param name="x1">The x1 value.</param>
  /// <param name="y1">The y1 value.</param>
  /// <param name="x2">The x2 value.</param>
  /// <param name="y2">The y2 value.</param>
  /// <param name="color">The color value.</param>
  public void DrawLine(int x1, int y1, int x2, int y2, Rgba color)
  {
    int dx = Math.Abs(x2 - x1), dy = Math.Abs(y2 - y1);
    int sx = x1 < x2 ? 1 : -1;
    int sy = y1 < y2 ? 1 : -1;
    int err = dx - dy;

    while (true)
    {
      SetPixel(x1, y1, color);
      if (x1 == x2 && y1 == y2) break;
      int e2 = 2 * err;
      if (e2 > -dy) { err -= dy; x1 += sx; }
      if (e2 < dx) { err += dx; y1 += sy; }
    }
  }

  /// <summary>
  /// Fill rect.
  /// </summary>
  /// <param name="px">The px value.</param>
  /// <param name="py">The py value.</param>
  /// <param name="pw">The pw value.</param>
  /// <param name="ph">The ph value.</param>
  /// <param name="color">The color value.</param>
  public void FillRect(int px, int py, int pw, int ph, Rgba color)
  {
    for (int y = py; y < py + ph; y++)
      for (int x = px; x < px + pw; x++)
        SetPixel(x, y, color);
  }

  /// <summary>
  /// Draw circle.
  /// </summary>
  /// <param name="cx">The cx value.</param>
  /// <param name="cy">The cy value.</param>
  /// <param name="r">The r value.</param>
  /// <param name="color">The color value.</param>
  public void DrawCircle(int cx, int cy, int r, Rgba color)
  {
    for (float angle = 0; angle < 2 * MathF.PI; angle += 0.01f)
    {
      int px = (int)(cx + r * MathF.Cos(angle));
      int py = (int)(cy + r * MathF.Sin(angle));
      SetPixel(px, py, color);
    }
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int sx = ScreenX, sy = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0 || BufferWidth <= 0 || BufferHeight <= 0) return;

    // Each cell is 2 pixel rows (top=fg with ▀, bottom=bg)
    for (int cy = 0; cy < h; cy++)
    {
      for (int cx = 0; cx < w; cx++)
      {
        int px = Math.Clamp(cx * BufferWidth / Math.Max(1, w), 0, BufferWidth - 1);
        int py1 = Math.Clamp((cy * 2) * BufferHeight / Math.Max(1, h * 2), 0, BufferHeight - 1);
        int py2 = Math.Clamp((cy * 2 + 1) * BufferHeight / Math.Max(1, h * 2), 0, BufferHeight - 1);

        var topColor = _pixels[py1 * BufferWidth + px];
        var botColor = _pixels[py2 * BufferWidth + px];

        buffer.SetCell(sx + cx, sy + cy, '▀', topColor, botColor);
      }
    }
  }
}
