using TermBlade.Core.Ansi;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents text options.
/// </summary>
public class TextOptions
{
  /// <summary>
  /// Gets or sets the content.
  /// </summary>
  public string Content { get; set; } = "";
  /// <summary>
  /// Gets or sets the fg.
  /// </summary>
  public string? Fg { get; set; }
  /// <summary>
  /// Gets or sets the bg.
  /// </summary>
  public string? Bg { get; set; }
  /// <summary>
  /// Gets or sets the text align.
  /// </summary>
  public string TextAlign { get; set; } = "left";
  /// <summary>
  /// Gets or sets the wrap.
  /// </summary>
  public bool Wrap { get; set; } = true;
  /// <summary>
  /// Gets or sets the attributes.
  /// </summary>
  public TextAttributes Attributes { get; set; } = TextAttributes.None;
  /// <summary>
  /// Gets or sets the width.
  /// </summary>
  public object? Width { get; set; }
  /// <summary>
  /// Gets or sets the height.
  /// </summary>
  public object? Height { get; set; }
  /// <summary>
  /// Gets or sets the flex grow.
  /// </summary>
  public float FlexGrow { get; set; } = 0;
}

/// <summary>
/// Represents text renderable.
/// </summary>
public class TextRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the content.
  /// </summary>
  public string Content { get; set; }
  /// <summary>
  /// Gets or sets the fg.
  /// </summary>
  public string? Fg { get; set; }
  /// <summary>
  /// Gets or sets the bg.
  /// </summary>
  public string? Bg { get; set; }
  /// <summary>
  /// Gets or sets the text align.
  /// </summary>
  public string TextAlign { get; set; }
  /// <summary>
  /// Gets or sets the wrap.
  /// </summary>
  public bool Wrap { get; set; }
  /// <summary>
  /// Gets or sets the attributes.
  /// </summary>
  public TextAttributes Attributes { get; set; }

  /// <summary>
  /// Text renderable.
  /// </summary>
  /// <param name="renderer">The renderer value.</param>
  /// <param name="options">The options value.</param>
  public TextRenderable(CliRenderer? renderer, TextOptions? options = null) : base(renderer)
  {
    var opts = options ?? new TextOptions();
    Content = opts.Content;
    Fg = opts.Fg;
    Bg = opts.Bg;
    TextAlign = opts.TextAlign;
    Wrap = opts.Wrap;
    Attributes = opts.Attributes;
    LayoutNode.FlexGrow = opts.FlexGrow;
    if (opts.Width != null) SetInitialWidth(opts.Width);
    if (opts.Height != null) SetInitialHeight(opts.Height);
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(255, 255, 255);
    var bg = Bg != null ? Rgba.FromCss(Bg) : Rgba.FromValues(0, 0, 0, 0);

    var lines = Wrap ? WrapText(Content, w) : Content.Split('\n').ToList();

    for (int i = 0; i < lines.Count && i < h; i++)
    {
      var line = lines[i];
      int drawX = x;
      if (TextAlign == "center")
        drawX = x + Math.Max(0, (w - line.Length) / 2);
      else if (TextAlign == "right")
        drawX = x + Math.Max(0, w - line.Length);

      buffer.DrawText(drawX, y + i, line, fg, bg, Attributes);
    }
  }

  private static List<string> WrapText(string text, int maxWidth)
  {
    var result = new List<string>();
    if (maxWidth <= 0) return result;

    var paragraphs = text.Split('\n');
    foreach (var para in paragraphs)
    {
      if (para.Length <= maxWidth)
      {
        result.Add(para);
        continue;
      }

      var words = para.Split(' ');
      var current = new System.Text.StringBuilder();
      foreach (var word in words)
      {
        if (current.Length == 0)
        {
          current.Append(word);
        }
        else if (current.Length + 1 + word.Length <= maxWidth)
        {
          current.Append(' ');
          current.Append(word);
        }
        else
        {
          result.Add(current.ToString());
          current.Clear();
          current.Append(word);
        }
      }
      if (current.Length > 0) result.Add(current.ToString());
    }
    return result;
  }
}
