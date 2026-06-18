using TermBlade.Core.Ansi;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents box options.
/// </summary>
public class BoxOptions
{
  /// <summary>
  /// Gets or sets the width.
  /// </summary>
  public object? Width { get; set; }
  /// <summary>
  /// Gets or sets the height.
  /// </summary>
  public object? Height { get; set; }
  /// <summary>
  /// Gets or sets the background color.
  /// </summary>
  public string BackgroundColor { get; set; } = "transparent";
  /// <summary>
  /// Gets or sets the border.
  /// </summary>
  public bool Border { get; set; } = false;
  /// <summary>
  /// Gets or sets the border style.
  /// </summary>
  public string BorderStyle { get; set; } = "single";
  /// <summary>
  /// Gets or sets the border color.
  /// </summary>
  public string BorderColor { get; set; } = "#ffffff";
  /// <summary>
  /// Gets or sets the focused border color.
  /// </summary>
  public string? FocusedBorderColor { get; set; }
  /// <summary>
  /// Gets or sets the should fill.
  /// </summary>
  public bool ShouldFill { get; set; } = true;
  /// <summary>
  /// Gets or sets the title.
  /// </summary>
  public string? Title { get; set; }
  /// <summary>
  /// Gets or sets the title alignment.
  /// </summary>
  public string TitleAlignment { get; set; } = "left";
  /// <summary>
  /// Gets or sets the bottom title.
  /// </summary>
  public string? BottomTitle { get; set; }
  /// <summary>
  /// Gets or sets the bottom title alignment.
  /// </summary>
  public string BottomTitleAlignment { get; set; } = "left";
  /// <summary>
  /// Gets or sets the flex direction.
  /// </summary>
  public FlexDirection FlexDirection { get; set; } = FlexDirection.Column;
  /// <summary>
  /// Gets or sets the align items.
  /// </summary>
  public AlignItems AlignItems { get; set; } = AlignItems.FlexStart;
  /// <summary>
  /// Gets or sets the justify content.
  /// </summary>
  public JustifyContent JustifyContent { get; set; } = JustifyContent.FlexStart;
  /// <summary>
  /// Gets or sets the flex grow.
  /// </summary>
  public float FlexGrow { get; set; } = 0;
  /// <summary>
  /// Gets or sets the padding top.
  /// </summary>
  public int PaddingTop { get; set; }
  /// <summary>
  /// Gets or sets the padding right.
  /// </summary>
  public int PaddingRight { get; set; }
  /// <summary>
  /// Gets or sets the padding bottom.
  /// </summary>
  public int PaddingBottom { get; set; }
  /// <summary>
  /// Gets or sets the padding left.
  /// </summary>
  public int PaddingLeft { get; set; }
}

/// <summary>
/// Represents box renderable.
/// </summary>
public class BoxRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the background color.
  /// </summary>
  public string BackgroundColor { get; set; }
  /// <summary>
  /// Gets or sets the border.
  /// </summary>
  public bool Border { get; set; }
  /// <summary>
  /// Gets or sets the border style.
  /// </summary>
  public string BorderStyle { get; set; }
  /// <summary>
  /// Gets or sets the border color.
  /// </summary>
  public string BorderColor { get; set; }
  /// <summary>
  /// Gets or sets the focused border color.
  /// </summary>
  public string? FocusedBorderColor { get; set; }
  /// <summary>
  /// Gets or sets the should fill.
  /// </summary>
  public bool ShouldFill { get; set; }
  /// <summary>
  /// Gets or sets the title.
  /// </summary>
  public string? Title { get; set; }
  /// <summary>
  /// Gets or sets the title alignment.
  /// </summary>
  public string TitleAlignment { get; set; }
  /// <summary>
  /// Gets or sets the bottom title.
  /// </summary>
  public string? BottomTitle { get; set; }
  /// <summary>
  /// Gets or sets the bottom title alignment.
  /// </summary>
  public string BottomTitleAlignment { get; set; }

  /// <summary>
  /// Box renderable.
  /// </summary>
  /// <param name="renderer">The renderer value.</param>
  /// <param name="options">The options value.</param>
  public BoxRenderable(CliRenderer? renderer, BoxOptions? options = null) : base(renderer)
  {
    var opts = options ?? new BoxOptions();
    BackgroundColor = opts.BackgroundColor;
    Border = opts.Border;
    BorderStyle = opts.BorderStyle;
    BorderColor = opts.BorderColor;
    FocusedBorderColor = opts.FocusedBorderColor;
    ShouldFill = opts.ShouldFill;
    Title = opts.Title;
    TitleAlignment = opts.TitleAlignment;
    BottomTitle = opts.BottomTitle;
    BottomTitleAlignment = opts.BottomTitleAlignment;

    LayoutNode.FlexDirection = opts.FlexDirection;
    LayoutNode.AlignItems = opts.AlignItems;
    LayoutNode.JustifyContent = opts.JustifyContent;
    LayoutNode.FlexGrow = opts.FlexGrow;
    LayoutNode.PaddingTop = opts.PaddingTop;
    LayoutNode.PaddingRight = opts.PaddingRight;
    LayoutNode.PaddingBottom = opts.PaddingBottom;
    LayoutNode.PaddingLeft = opts.PaddingLeft;

    if (opts.Width != null) SetInitialWidth(opts.Width);
    if (opts.Height != null) SetInitialHeight(opts.Height);

    if (Border)
    {
      LayoutNode.PaddingTop = Math.Max(1, LayoutNode.PaddingTop);
      LayoutNode.PaddingRight = Math.Max(1, LayoutNode.PaddingRight);
      LayoutNode.PaddingBottom = Math.Max(1, LayoutNode.PaddingBottom);
      LayoutNode.PaddingLeft = Math.Max(1, LayoutNode.PaddingLeft);
    }

    Focusable = FocusedBorderColor != null;
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var bg = ParseColor(BackgroundColor);

    if (ShouldFill && bg.AlphaByte > 0)
      buffer.FillRect(x, y, w, h, bg);

    if (Border)
    {
      var borderRgba = ParseBorderColor();
      buffer.DrawBorderWithTitles(x, y, w, h, BorderStyle, borderRgba, bg,
          Title, TitleAlignment, BottomTitle, BottomTitleAlignment);
    }
  }

  private static Rgba ParseColor(string color)
  {
    if (string.IsNullOrEmpty(color) || color == "transparent")
      return Rgba.FromValues(0, 0, 0, 0);
    return Rgba.FromCss(color);
  }

  private Rgba ParseBorderColor()
  {
    var colorStr = (Focused && FocusedBorderColor != null) ? FocusedBorderColor : BorderColor;
    return ParseColor(colorStr);
  }
}
