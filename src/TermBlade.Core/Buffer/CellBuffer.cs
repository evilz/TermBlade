using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TermBlade.Core.Ansi;

namespace TermBlade.Core.Buffer
{
  /// <summary>
  /// A 2D grid of terminal cells. Pure C# port of the Zig <c>OptimizedBuffer</c>.
  ///
  /// Supports drawing text (ASCII + Unicode graphemes), box borders, alpha blending,
  /// frame-buffer blitting and resize — enough to drive a full TUI layout.
  /// </summary>
  public sealed class CellBuffer : IDisposable
  {
    private static int _idSeq;

    /// <summary>
    /// Gets the id.
    /// </summary>
    public string Id { get; }
    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public int Width { get; private set; }
    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public int Height { get; private set; }
    /// <summary>
    /// Gets or sets the respect alpha.
    /// </summary>
    public bool RespectAlpha { get; set; }

    private Cell[] _cells;
    private bool _disposed;

    /// <summary>
    /// Cell buffer.
    /// </summary>
    /// <param name="width">The width value.</param>
    /// <param name="height">The height value.</param>
    /// <param name="id">The id value.</param>
    /// <param name="respectAlpha">The respectAlpha value.</param>
    public CellBuffer(int width, int height, string? id = null, bool respectAlpha = false)
    {
      if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
      if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
      Id = id ?? $"fb_{System.Threading.Interlocked.Increment(ref _idSeq)}";
      Width = width;
      Height = height;
      RespectAlpha = respectAlpha;
      _cells = new Cell[width * height];
      FillAll(Rgba.FromInts(0, 0, 0));
    }

    /// <summary>
    /// Create.
    /// </summary>
    /// <param name="width">The width value.</param>
    /// <param name="height">The height value.</param>
    public static CellBuffer Create(int width, int height, string? id = null, bool respectAlpha = false)
        => new CellBuffer(width, height, id, respectAlpha);

    // ── guard ─────────────────────────────────────────────────────────────────

    private void Guard()
    {
      if (_disposed) throw new ObjectDisposedException(Id);
    }

    // ── fill ──────────────────────────────────────────────────────────────────

    private void FillAll(Rgba bg)
    {
      var empty = Cell.Empty(bg);
      for (int i = 0; i < _cells.Length; i++) _cells[i] = empty;
    }

    /// <summary>
    /// Clear.
    /// </summary>
    public void Clear(Rgba? bg = null)
    {
      Guard();
      FillAll(bg ?? Rgba.FromInts(0, 0, 0));
    }

    // ── bounds ────────────────────────────────────────────────────────────────

    private bool InBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;

    // ── cell access ───────────────────────────────────────────────────────────

    /// <summary>
    /// Get cell.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    public Cell? GetCell(int x, int y)
    {
      Guard();
      if (!InBounds(x, y)) return null;
      return _cells[y * Width + x];
    }

    /// <summary>
    /// Set cell.
    /// </summary>
    public void SetCell(int x, int y, int codepoint, Rgba fg, Rgba bg,
                        TextAttributes attrs = TextAttributes.None)
    {
      Guard();
      if (!InBounds(x, y)) return;
      _cells[y * Width + x] = new Cell { Codepoint = codepoint, Fg = fg, Bg = bg, Attributes = attrs };
    }

    private void SetGrapheme(int x, int y, ReadOnlySpan<char> grapheme, Rgba fg, Rgba bg,
                             TextAttributes attrs)
    {
      if (!InBounds(x, y)) return;
      var runes = grapheme.EnumerateRunes();
      if (!runes.MoveNext()) return;
      var firstRune = runes.Current;
      var hasMultipleRunes = runes.MoveNext();
      _cells[y * Width + x] = new Cell
      {
        Codepoint = firstRune.Value,
        Grapheme = hasMultipleRunes ? grapheme.ToString() : null,
        Fg = fg,
        Bg = bg,
        Attributes = attrs,
      };
    }

    // ── draw text ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Draw text.
    /// </summary>
    public void DrawText(string text, int x, int y, Rgba fg, Rgba? bg = null,
                         TextAttributes attrs = TextAttributes.None)
    {
      Guard();
      var bgColor = bg ?? Rgba.FromInts(0, 0, 0, 0);
      int col = x;
      var remaining = text.AsSpan();
      while (!remaining.IsEmpty)
      {
        if (col >= Width) break;
        int graphemeLength = StringInfo.GetNextTextElementLength(remaining);
        var grapheme = remaining[..graphemeLength];
        int w = GraphemeWidth(grapheme);
        if (w == 0)
        {
          remaining = remaining[graphemeLength..];
          continue;
        }
        if (col < 0)
        {
          col += w;
          remaining = remaining[graphemeLength..];
          continue;
        }
        if (w > 1 && col + w > Width) break;
        SetGrapheme(col, y, grapheme, fg, bgColor, attrs);
        for (int continuation = 1; continuation < w; continuation++)
          SetCell(col + continuation, y, 0, fg, bgColor, attrs);
        col += w;
        remaining = remaining[graphemeLength..];
      }
    }

    // ── draw box ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Draw box.
    /// </summary>
    public void DrawBox(int x, int y, int width, int height,
                        Rgba borderColor, Rgba bg,
                        BorderStyle style = BorderStyle.Single,
                        BorderSides sides = BorderSides.All,
                        bool fill = false,
                        string? title = null,
                        string? bottomTitle = null,
                        TextAlignment titleAlign = TextAlignment.Left,
                        TextAlignment bottomTitleAlign = TextAlignment.Left)
    {
      Guard();
      var chars = BorderChars.GetChars(style);

      if (fill)
        for (int fy = y + 1; fy < y + height - 1; fy++)
          for (int fx = x + 1; fx < x + width - 1; fx++)
            SetCell(fx, fy, ' ', borderColor, bg);

      bool top = (sides & BorderSides.Top) != 0;
      bool bot = (sides & BorderSides.Bottom) != 0;
      bool lft = (sides & BorderSides.Left) != 0;
      bool rgt = (sides & BorderSides.Right) != 0;

      if (top && lft) SetCell(x, y, chars[0], borderColor, bg);
      if (top && rgt) SetCell(x + width - 1, y, chars[1], borderColor, bg);
      if (bot && lft) SetCell(x, y + height - 1, chars[2], borderColor, bg);
      if (bot && rgt) SetCell(x + width - 1, y + height - 1, chars[3], borderColor, bg);

      for (int fx = x + 1; fx < x + width - 1; fx++)
      {
        if (top) SetCell(fx, y, chars[4], borderColor, bg);
        if (bot) SetCell(fx, y + height - 1, chars[4], borderColor, bg);
      }

      for (int fy = y + 1; fy < y + height - 1; fy++)
      {
        if (lft) SetCell(x, fy, chars[5], borderColor, bg);
        if (rgt) SetCell(x + width - 1, fy, chars[5], borderColor, bg);
      }

      if (title != null && top && width > 4)
      {
        var t = TruncateToCellWidth(title, width - 4);
        int titleWidth = GetTextWidth(t.AsSpan());
        int tx = titleAlign switch
        {
          TextAlignment.Center => x + (width - titleWidth) / 2,
          TextAlignment.Right => x + width - titleWidth - 2,
          _ => x + 2,
        };
        DrawText(t, tx, y, borderColor, bg);
      }

      if (bottomTitle != null && bot && width > 4)
      {
        var t = TruncateToCellWidth(bottomTitle, width - 4);
        int titleWidth = GetTextWidth(t.AsSpan());
        int tx = bottomTitleAlign switch
        {
          TextAlignment.Center => x + (width - titleWidth) / 2,
          TextAlignment.Right => x + width - titleWidth - 2,
          _ => x + 2,
        };
        DrawText(t, tx, y + height - 1, borderColor, bg);
      }
    }

    /// <summary>
    /// Fill rect.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    /// <param name="w">The w value.</param>
    /// <param name="h">The h value.</param>
    /// <param name="bg">The bg value.</param>
    public void FillRect(int x, int y, int w, int h, Rgba bg)
    {
      Guard();
      for (int fy = y; fy < y + h; fy++)
        for (int fx = x; fx < x + w; fx++)
          SetCell(fx, fy, ' ', Rgba.FromInts(255, 255, 255), bg);
    }

    // ── blit (DrawFrameBuffer) ────────────────────────────────────────────────

    /// <summary>
    /// Blit <paramref name="src"/> onto this buffer at the given destination position.
    /// If <see cref="RespectAlpha"/> is set, semi-transparent source cells are blended.
    /// </summary>
    public void DrawFrameBuffer(int destX, int destY, CellBuffer src,
                                int srcX = 0, int srcY = 0,
                                int? srcWidth = null, int? srcHeight = null)
    {
      Guard();
      int sw = srcWidth ?? src.Width;
      int sh = srcHeight ?? src.Height;
      for (int fy = 0; fy < sh; fy++)
        for (int fx = 0; fx < sw; fx++)
        {
          int dx = destX + fx, dy = destY + fy;
          if (!InBounds(dx, dy)) continue;
          var cell = src.GetCell(srcX + fx, srcY + fy);
          if (cell == null) continue;
          var c = cell.Value;

          // Copy a wide cluster as one unit so a source crop or destination edge
          // cannot leave only its leading cell visible.
          if (c.Codepoint == 0)
          {
            if (RespectAlpha && c.Bg.AlphaByte < 255)
            {
              var existing = _cells[dy * Width + dx];
              c.Bg = c.Bg.BlendOver(existing.Bg);
            }
            _cells[dy * Width + dx] = c;
            continue;
          }

          int cellWidth = GetCellWidth(c);
          if (cellWidth > 1)
          {
            if (fx + cellWidth > sw || srcX + fx + cellWidth > src.Width ||
                dx + cellWidth > Width || !InBounds(dx + cellWidth - 1, dy))
            {
              fx += cellWidth - 1;
              continue;
            }
          }

          if (RespectAlpha && c.Bg.AlphaByte < 255)
          {
            var existing = _cells[dy * Width + dx];
            c.Bg = c.Bg.BlendOver(existing.Bg);
          }
          _cells[dy * Width + dx] = c;

          for (int continuation = 1; continuation < cellWidth; continuation++)
          {
            var continuationCell = src.GetCell(srcX + fx + continuation, srcY + fy);
            if (continuationCell == null) break;
            var continuationValue = continuationCell.Value;
            if (RespectAlpha && continuationValue.Bg.AlphaByte < 255)
            {
              var existing = _cells[dy * Width + dx + continuation];
              continuationValue.Bg = continuationValue.Bg.BlendOver(existing.Bg);
            }
            _cells[dy * Width + dx + continuation] = continuationValue;
          }
          fx += Math.Max(0, cellWidth - 1);
        }
    }

    // ── resize ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Resize.
    /// </summary>
    /// <param name="width">The width value.</param>
    /// <param name="height">The height value.</param>
    public void Resize(int width, int height)
    {
      Guard();
      if (Width == width && Height == height) return;
      var newCells = new Cell[width * height];
      var empty = Cell.Empty(Rgba.FromInts(0, 0, 0));
      for (int i = 0; i < newCells.Length; i++) newCells[i] = empty;
      for (int fy = 0; fy < Math.Min(Height, height); fy++)
        for (int fx = 0; fx < Math.Min(Width, width); fx++)
        {
          var cell = _cells[fy * Width + fx];
          if (cell.Codepoint != 0 && fx + GetCellWidth(cell) > width)
            continue;

          newCells[fy * width + fx] = cell;
        }
      _cells = newCells;
      Width = width;
      Height = height;
    }

    // ── encode unicode ────────────────────────────────────────────────────────

    /// <summary>
    /// Decompose <paramref name="text"/> into (displayWidth, codepoint) pairs,
    /// mirroring the Zig <c>encodeUnicode</c> / <c>GraphemePool</c> API.
    /// </summary>
    public List<(int Width, int Codepoint)> EncodeUnicode(string text)
    {
      Guard();
      var result = new List<(int, int)>(text.Length);
      foreach (var rune in text.EnumerateRunes())
        result.Add((RuneWidth(rune), rune.Value));
      return result;
    }

    // ── byte snapshot ─────────────────────────────────────────────────────────

    /// <summary>Return UTF-8 bytes for all visible characters in the buffer.</summary>
    public byte[] GetRealCharBytes(bool addLineBreaks = false)
    {
      Guard();
      var sb = new StringBuilder(Width * Height);
      for (int fy = 0; fy < Height; fy++)
      {
        for (int fx = 0; fx < Width; fx++)
        {
          var cell = _cells[fy * Width + fx];
          if (cell.Codepoint == 0) continue; // continuation cell
          sb.Append(cell.Grapheme ?? char.ConvertFromUtf32(cell.Codepoint));
        }
        if (addLineBreaks && fy < Height - 1) sb.Append('\n');
      }
      return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // ── grapheme width detection ──────────────────────────────────────────────

    /// <summary>
    /// Returns the approximate terminal width of one Unicode scalar: zero, one, or two columns.
    /// </summary>
    /// <param name="rune">The Unicode scalar to measure.</param>
    /// <returns>The approximate number of terminal columns occupied by the scalar.</returns>
    public static int RuneWidth(System.Text.Rune rune)
    {
      int v = rune.Value;
      var category = Rune.GetUnicodeCategory(rune);
      if (category is UnicodeCategory.NonSpacingMark or UnicodeCategory.EnclosingMark or
          UnicodeCategory.SpacingCombiningMark or
          UnicodeCategory.Format or UnicodeCategory.Control)
        return 0;
      if (v < 0x1100) return 1;
      if (v <= 0x115F ||    // Hangul Jamo
          v == 0x2329 || v == 0x232A ||
          (v >= 0x2600 && v <= 0x27BF) ||
          (v >= 0x2E80 && v <= 0x303E) ||
          (v >= 0x3040 && v <= 0xA4CF) ||
          (v >= 0xAC00 && v <= 0xD7AF) ||
          (v >= 0xF900 && v <= 0xFAFF) ||
          (v >= 0xFE10 && v <= 0xFE19) ||
          (v >= 0xFE30 && v <= 0xFE4F) ||
          (v >= 0xFF00 && v <= 0xFF60) ||
          (v >= 0xFFE0 && v <= 0xFFE6) ||
          (v >= 0x1F004 && v <= 0x1FFFD) ||   // emoji + misc symbols
          (v >= 0x20000 && v <= 0x2FFFD) ||
          (v >= 0x30000 && v <= 0x3FFFD))
        return 2;
      return 1;
    }

    /// <summary>Returns the approximate terminal width of exactly one Unicode grapheme cluster.</summary>
    /// <param name="grapheme">A string containing exactly one grapheme cluster.</param>
    /// <returns>The approximate number of terminal columns occupied by the cluster.</returns>
    /// <exception cref="ArgumentException">The value is empty or contains more than one grapheme cluster.</exception>
    public static int GraphemeWidth(string grapheme)
    {
      ArgumentException.ThrowIfNullOrEmpty(grapheme);
      var text = grapheme.AsSpan();
      if (StringInfo.GetNextTextElementLength(text) != text.Length)
        throw new ArgumentException("The value must contain exactly one grapheme cluster.", nameof(grapheme));

      return GraphemeWidth(text);
    }

    internal static int GraphemeWidth(ReadOnlySpan<char> grapheme)
    {
      int width = 0;
      int regionalIndicators = 0;
      int spacingCharacters = 0;
      bool onlyRegionalIndicators = true;
      bool onlyHangulJamo = true;
      int emojiBaseCount = 0;
      bool hasKeycapBase = false;
      bool hasJoiner = false;
      bool hasEmojiVariationSelector = false;
      bool hasTextVariationSelector = false;
      bool hasEmojiModifier = false;
      bool hasKeycap = false;

      foreach (var rune in grapheme.EnumerateRunes())
      {
        int value = rune.Value;
        hasJoiner |= value == 0x200D;
        hasEmojiVariationSelector |= value == 0xFE0F;
        hasTextVariationSelector |= value == 0xFE0E;
        hasEmojiModifier |= value >= 0x1F3FB && value <= 0x1F3FF;
        hasKeycap |= value == 0x20E3;
        if (IsEmojiBase(value) && (value < 0x1F3FB || value > 0x1F3FF))
          emojiBaseCount++;
        hasKeycapBase |= value == 0x23 || value == 0x2A || (value >= 0x30 && value <= 0x39);

        if (value >= 0x1F1E6 && value <= 0x1F1FF)
          regionalIndicators++;
        else
          onlyRegionalIndicators = false;

        var category = Rune.GetUnicodeCategory(rune);
        if (!IsHangulJamo(value) && category is not
            (UnicodeCategory.NonSpacingMark or UnicodeCategory.EnclosingMark or
             UnicodeCategory.SpacingCombiningMark or UnicodeCategory.Format))
          onlyHangulJamo = false;

        int runeWidth = RuneWidth(rune);
        if (runeWidth > 0)
        {
          spacingCharacters++;
          width += runeWidth;
        }
      }

      if (onlyRegionalIndicators && regionalIndicators > 1)
        return 2;
      if (hasTextVariationSelector && emojiBaseCount > 0 && !hasKeycap)
        return 1;
      if ((hasEmojiVariationSelector || hasEmojiModifier) && emojiBaseCount > 0)
        return 2;
      if (hasJoiner && emojiBaseCount > 1)
        return 2;
      if (hasKeycapBase && (hasEmojiVariationSelector || hasKeycap))
        return 2;
      if (onlyHangulJamo && spacingCharacters > 1)
        return 2;

      return width;
    }

    private static int GetCellWidth(Cell cell)
    {
      if (cell.Codepoint == 0)
        return 0;
      if (cell.Grapheme != null)
        return GraphemeWidth(cell.Grapheme);
      return Rune.TryCreate(cell.Codepoint, out var rune) ? RuneWidth(rune) : 1;
    }

    internal static int GetTextWidth(ReadOnlySpan<char> text)
    {
      int width = 0;
      while (!text.IsEmpty)
      {
        int graphemeLength = StringInfo.GetNextTextElementLength(text);
        width += GraphemeWidth(text[..graphemeLength]);
        text = text[graphemeLength..];
      }

      return width;
    }

    internal static string TruncateToCellWidth(string text, int maxWidth)
    {
      if (maxWidth <= 0 || string.IsNullOrEmpty(text))
        return string.Empty;

      int width = 0;
      int length = 0;
      var remaining = text.AsSpan();
      while (!remaining.IsEmpty)
      {
        int graphemeLength = StringInfo.GetNextTextElementLength(remaining);
        int graphemeWidth = GraphemeWidth(remaining[..graphemeLength]);
        if (width + graphemeWidth > maxWidth)
          break;

        width += graphemeWidth;
        length += graphemeLength;
        remaining = remaining[graphemeLength..];
      }

      return length == text.Length ? text : text[..length];
    }

    private static bool IsEmojiBase(int value)
        => (value >= 0x1F000 && value <= 0x1FAFF)
            || (value >= 0x2600 && value <= 0x27BF)
            || value is 0x00A9 or 0x00AE or 0x203C or 0x2049 or 0x2122 or 0x2139
                or 0x2194 or 0x2195 or 0x2196 or 0x2197 or 0x2198 or 0x2199 or 0x21A9 or 0x21AA
                or 0x231A or 0x231B or 0x2328 or 0x23CF
                or 0x23E9 or 0x23EA or 0x23EB or 0x23EC or 0x23ED or 0x23EE or 0x23EF
                or 0x23F0 or 0x23F1 or 0x23F2 or 0x23F3 or 0x23F8 or 0x23F9 or 0x23FA
                or 0x24C2 or 0x25AA or 0x25AB or 0x25B6 or 0x25C0
                or 0x2934 or 0x2935 or 0x3030 or 0x303D or 0x3297 or 0x3299;

    private static bool IsHangulJamo(int value)
        => (value >= 0x1100 && value <= 0x11FF)
            || (value >= 0xA960 && value <= 0xA97F)
            || (value >= 0xD7B0 && value <= 0xD7FF);

    // ── IDisposable ───────────────────────────────────────────────────────────

    /// <summary>
    /// Dispose.
    /// </summary>
    public void Dispose() { _disposed = true; }
  }
}
