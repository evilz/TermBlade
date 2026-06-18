using System.Text;
using System.Text.RegularExpressions;
using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents markdown renderable.
/// </summary>
public class MarkdownRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the content.
  /// </summary>
  public string Content { get; set; } = "";

  private static readonly Rgba H1Color = Rgba.FromCss("#61afef");
  private static readonly Rgba H2Color = Rgba.FromCss("#e5c07b");
  private static readonly Rgba H3Color = Rgba.FromCss("#98c379");
  private static readonly Rgba CodeBg = Rgba.FromInts(40, 40, 40);
  private static readonly Rgba DefaultFg = Rgba.FromInts(212, 212, 212);
  private static readonly Rgba BulletFg = Rgba.FromCss("#e06c75");
  private static readonly Rgba DefaultBg = Rgba.FromInts(0, 0, 0, 0);
  private static readonly Rgba LinkColor = Rgba.FromCss("#56b6c2");
  private static readonly Rgba BlockquoteFg = Rgba.FromInts(150, 150, 150);

  /// <summary>
  /// Markdown renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public MarkdownRenderable(CliRenderer? renderer) : base(renderer) { }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var lines = Content.Split('\n');
    int row = 0;
    bool inCodeBlock = false;

    for (int i = 0; i < lines.Length && row < h; i++)
    {
      var line = lines[i];

      // Code block fence
      if (line.TrimStart().StartsWith("```"))
      {
        inCodeBlock = !inCodeBlock;
        continue;
      }

      if (inCodeBlock)
      {
        buffer.FillRect(x, y + row, w, 1, CodeBg);
        DrawTextClipped(buffer, x, y + row, w, line, DefaultFg, CodeBg);
        row++;
        continue;
      }

      // Headings
      if (line.StartsWith("### "))
      {
        RenderLine(buffer, x, y + row, w, line[4..], H3Color, DefaultBg, TextAttributes.Bold);
        row++;
        continue;
      }
      if (line.StartsWith("## "))
      {
        RenderLine(buffer, x, y + row, w, line[3..], H2Color, DefaultBg, TextAttributes.Bold);
        row++;
        continue;
      }
      if (line.StartsWith("# "))
      {
        RenderLine(buffer, x, y + row, w, line[2..], H1Color, DefaultBg, TextAttributes.Bold);
        row++;
        continue;
      }

      // Blockquote
      if (line.StartsWith("> "))
      {
        buffer.DrawText(x, y + row, "│ ", BlockquoteFg, DefaultBg);
        RenderInline(buffer, x + 2, y + row, w - 2, line[2..]);
        row++;
        continue;
      }

      // Horizontal rule
      if (Regex.IsMatch(line, @"^[-*_]{3,}\s*$"))
      {
        var hr = new string('─', w);
        buffer.DrawText(x, y + row, hr, Rgba.FromInts(100, 100, 100), DefaultBg);
        row++;
        continue;
      }

      // Bullet list
      if (line.StartsWith("- ") || line.StartsWith("* ") || line.StartsWith("+ "))
      {
        buffer.DrawText(x, y + row, "• ", BulletFg, DefaultBg);
        RenderInline(buffer, x + 2, y + row, w - 2, line[2..]);
        row++;
        continue;
      }

      // Numbered list
      var numberedMatch = Regex.Match(line, @"^(\d+)\.\s+(.*)");
      if (numberedMatch.Success)
      {
        var num = numberedMatch.Groups[1].Value + ". ";
        buffer.DrawText(x, y + row, num, BulletFg, DefaultBg);
        RenderInline(buffer, x + num.Length, y + row, w - num.Length, numberedMatch.Groups[2].Value);
        row++;
        continue;
      }

      // Regular paragraph
      RenderInline(buffer, x, y + row, w, line);
      row++;
    }
  }

  private static void RenderLine(RenderBuffer buffer, int x, int y, int w, string text, Rgba fg, Rgba bg, TextAttributes attrs)
  {
    DrawTextClipped(buffer, x, y, w, text, fg, bg, attrs);
  }

  private static int DrawTextClipped(RenderBuffer buffer, int x, int y, int w, string text, Rgba fg, Rgba bg, TextAttributes attrs = TextAttributes.None)
  {
    if (w <= 0 || string.IsNullOrEmpty(text)) return 0;

    int col = 0;
    foreach (var rune in text.EnumerateRunes())
    {
      int runeWidth = CellBuffer.RuneWidth(rune);
      if (col + runeWidth > w)
        break;

      buffer.DrawText(x + col, y, rune.ToString(), fg, bg, attrs);
      col += runeWidth;
    }

    return col;
  }

  private static void RenderInline(RenderBuffer buffer, int x, int y, int w, string text)
  {
    if (w <= 0) return;
    // Parse inline markdown: **bold**, *italic*, `code`, [link](url)
    int col = 0;
    int i = 0;

    while (i < text.Length && col < w)
    {
      // Bold (**text**)
      if (i + 3 < text.Length && text[i] == '*' && text[i + 1] == '*')
      {
        int end = text.IndexOf("**", i + 2);
        if (end > 0)
        {
          var inner = text[(i + 2)..end];
          col += DrawTextClipped(buffer, x + col, y, w - col, inner, DefaultFg, DefaultBg, TextAttributes.Bold);
          i = end + 2;
          continue;
        }
      }

      // Italic (*text*)
      if (text[i] == '*' && (i + 1 >= text.Length || text[i + 1] != '*'))
      {
        int end = text.IndexOf('*', i + 1);
        if (end > 0)
        {
          var inner = text[(i + 1)..end];
          col += DrawTextClipped(buffer, x + col, y, w - col, inner, DefaultFg, DefaultBg, TextAttributes.Italic);
          i = end + 1;
          continue;
        }
      }

      // Inline code (`code`)
      if (text[i] == '`')
      {
        int end = text.IndexOf('`', i + 1);
        if (end > 0)
        {
          var inner = text[(i + 1)..end];
          col += DrawTextClipped(buffer, x + col, y, w - col, inner, Rgba.FromCss("#e06c75"), CodeBg);
          i = end + 1;
          continue;
        }
      }

      // Link [text](url)
      if (text[i] == '[')
      {
        var linkMatch = Regex.Match(text[i..], @"^\[([^\]]+)\]\([^\)]+\)");
        if (linkMatch.Success)
        {
          var linkText = linkMatch.Groups[1].Value;
          col += DrawTextClipped(buffer, x + col, y, w - col, linkText, LinkColor, DefaultBg, TextAttributes.Underline);
          i += linkMatch.Length;
          continue;
        }
      }

      // Regular Unicode scalar
      if (Rune.TryGetRuneAt(text, i, out var rune))
      {
        col += DrawTextClipped(buffer, x + col, y, w - col, rune.ToString(), DefaultFg, DefaultBg);
        i += rune.Utf16SequenceLength;
      }
      else
      {
        i++;
      }
    }
  }
}
