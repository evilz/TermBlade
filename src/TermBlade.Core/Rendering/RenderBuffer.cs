using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;

namespace TermBlade.Core.Rendering;

/// <summary>
/// Represents render buffer.
/// </summary>
public class RenderBuffer
{
  /// <summary>
  /// Gets the width.
  /// </summary>
  public int Width { get; }
  /// <summary>
  /// Gets the height.
  /// </summary>
  public int Height { get; }

  private readonly Cell[] _cells;
  private int? _clipX, _clipY, _clipW, _clipH;

  /// <summary>
  /// Render buffer.
  /// </summary>
  /// <param name="width">The width value.</param>
  /// <param name="height">The height value.</param>
  public RenderBuffer(int width, int height)
  {
    Width = width;
    Height = height;
    _cells = new Cell[width * height];
    var empty = Cell.Empty(Rgba.FromInts(0, 0, 0));
    for (int i = 0; i < _cells.Length; i++) _cells[i] = empty;
  }

  /// <summary>
  /// Set clip region.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  /// <param name="width">The width value.</param>
  /// <param name="height">The height value.</param>
  public void SetClipRegion(int? x, int? y, int? width, int? height)
  {
    _clipX = x; _clipY = y; _clipW = width; _clipH = height;
  }

  private bool InClip(int x, int y)
  {
    if (_clipX == null) return true;
    return x >= _clipX.Value && y >= _clipY!.Value &&
           x < _clipX.Value + _clipW!.Value &&
           y < _clipY.Value + _clipH!.Value;
  }

  private bool InBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;

  /// <summary>
  /// Set cell.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  /// <param name="ch">The ch value.</param>
  /// <param name="fg">The fg value.</param>
  /// <param name="bg">The bg value.</param>
  public void SetCell(int x, int y, char ch, Rgba fg, Rgba bg, TextAttributes attrs = 0)
      => SetCell(x, y, (int)ch, fg, bg, attrs);

  /// <summary>
  /// Set cell.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  /// <param name="codepoint">The codepoint value.</param>
  /// <param name="fg">The fg value.</param>
  /// <param name="bg">The bg value.</param>
  public void SetCell(int x, int y, int codepoint, Rgba fg, Rgba bg, TextAttributes attrs = 0)
  {
    if (!InBounds(x, y) || !InClip(x, y)) return;
    _cells[y * Width + x] = new Cell { Codepoint = codepoint, Fg = fg, Bg = bg, Attributes = attrs };
  }

  /// <summary>
  /// Draw text.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  /// <param name="text">The text value.</param>
  /// <param name="fg">The fg value.</param>
  /// <param name="bg">The bg value.</param>
  public void DrawText(int x, int y, string text, Rgba fg, Rgba bg, TextAttributes attrs = 0)
  {
    int col = x;
    foreach (var rune in text.EnumerateRunes())
    {
      if (col >= Width) break;
      int w = CellBuffer.RuneWidth(rune);
      SetCell(col, y, rune.Value, fg, bg, attrs);
      if (w == 2 && col + 1 < Width)
        SetCell(col + 1, y, 0, fg, bg, attrs);
      col += w;
    }
  }

  /// <summary>
  /// Fill rect.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  /// <param name="width">The width value.</param>
  /// <param name="height">The height value.</param>
  /// <param name="bg">The bg value.</param>
  public void FillRect(int x, int y, int width, int height, Rgba bg)
  {
    for (int fy = y; fy < y + height; fy++)
      for (int fx = x; fx < x + width; fx++)
        SetCell(fx, fy, ' ', Rgba.FromInts(255, 255, 255), bg);
  }

  /// <summary>
  /// Draw border.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  /// <param name="width">The width value.</param>
  /// <param name="height">The height value.</param>
  /// <param name="style">The style value.</param>
  /// <param name="color">The color value.</param>
  /// <param name="bg">The bg value.</param>
  public void DrawBorder(int x, int y, int width, int height, string style, Rgba color, Rgba bg)
  {
    if (width <= 0 || height <= 0) return;
    var chars = GetBorderChars(style);

    // Corners
    SetCell(x, y, chars[0], color, bg);
    if (width > 1) SetCell(x + width - 1, y, chars[1], color, bg);
    if (height > 1) SetCell(x, y + height - 1, chars[2], color, bg);
    if (width > 1 && height > 1) SetCell(x + width - 1, y + height - 1, chars[3], color, bg);

    // Top and bottom edges
    for (int fx = x + 1; fx < x + width - 1; fx++)
    {
      SetCell(fx, y, chars[4], color, bg);
      if (height > 1) SetCell(fx, y + height - 1, chars[4], color, bg);
    }

    // Left and right edges
    for (int fy = y + 1; fy < y + height - 1; fy++)
    {
      SetCell(x, fy, chars[5], color, bg);
      if (width > 1) SetCell(x + width - 1, fy, chars[5], color, bg);
    }
  }

  /// <summary>
  /// Draw border with titles.
  /// </summary>
  public void DrawBorderWithTitles(int x, int y, int width, int height, string style,
      Rgba color, Rgba bg, string? title, string titleAlign,
      string? bottomTitle, string bottomTitleAlign)
  {
    DrawBorder(x, y, width, height, style, color, bg);

    if (title != null && width > 4)
    {
      var t = title.Length > width - 4 ? title[..(width - 4)] : title;
      int tx = titleAlign switch
      {
        "center" => x + (width - t.Length) / 2,
        "right" => x + width - t.Length - 2,
        _ => x + 2
      };
      DrawText(tx, y, t, color, bg);
    }

    if (bottomTitle != null && width > 4 && height > 1)
    {
      var t = bottomTitle.Length > width - 4 ? bottomTitle[..(width - 4)] : bottomTitle;
      int tx = bottomTitleAlign switch
      {
        "center" => x + (width - t.Length) / 2,
        "right" => x + width - t.Length - 2,
        _ => x + 2
      };
      DrawText(tx, y + height - 1, t, color, bg);
    }
  }

  /// <summary>
  /// Get cell.
  /// </summary>
  /// <param name="x">The x value.</param>
  /// <param name="y">The y value.</param>
  public Cell? GetCell(int x, int y)
  {
    if (!InBounds(x, y)) return null;
    return _cells[y * Width + x];
  }

  /// <summary>
  /// Clear.
  /// </summary>
  /// <param name="bg">The bg value.</param>
  public void Clear(Rgba bg)
  {
    var empty = Cell.Empty(bg);
    for (int i = 0; i < _cells.Length; i++) _cells[i] = empty;
  }

  private static int[] GetBorderChars(string style) => style.ToLowerInvariant() switch
  {
    "double" => new[] { '╔', '╗', '╚', '╝', '═', '║' }.Select(c => (int)c).ToArray(),
    "rounded" or "spf" => new[] { '╭', '╮', '╰', '╯', '─', '│' }.Select(c => (int)c).ToArray(),
    "thick" or "heavy" => new[] { '┏', '┓', '┗', '┛', '━', '┃' }.Select(c => (int)c).ToArray(),
    "dashed" => new[] { '┌', '┐', '└', '┘', '╌', '╎' }.Select(c => (int)c).ToArray(),
    "ascii" => new[] { '+', '+', '+', '+', '-', '|' }.Select(c => (int)c).ToArray(),
    _ => new[] { '┌', '┐', '└', '┘', '─', '│' }.Select(c => (int)c).ToArray(), // single
  };
}
