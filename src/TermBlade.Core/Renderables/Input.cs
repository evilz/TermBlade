using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents input options.
/// </summary>
public class InputOptions
{
  /// <summary>
  /// Gets or sets the placeholder.
  /// </summary>
  public string? Placeholder { get; set; }
  /// <summary>
  /// Gets or sets the placeholder color.
  /// </summary>
  public string? PlaceholderColor { get; set; }
  /// <summary>
  /// Gets or sets the cursor color.
  /// </summary>
  public string? CursorColor { get; set; }
  /// <summary>
  /// Gets or sets the fg.
  /// </summary>
  public string? Fg { get; set; }
  /// <summary>
  /// Gets or sets the bg.
  /// </summary>
  public string? Bg { get; set; }
  /// <summary>
  /// Gets or sets the max length.
  /// </summary>
  public int? MaxLength { get; set; }
  /// <summary>
  /// Gets or sets the width.
  /// </summary>
  public object? Width { get; set; }
  /// <summary>
  /// Gets or sets the height.
  /// </summary>
  public object? Height { get; set; }
  /// <summary>
  /// Gets or sets the value.
  /// </summary>
  public string? Value { get; set; }
}

/// <summary>
/// Represents input renderable.
/// </summary>
public class InputRenderable : Renderable
{
  /// <summary>
  /// Member.
  /// </summary>
  public string Value
  {
    get;
    set
    {
      field = MaxLength.HasValue && value.Length > MaxLength.Value ? value[..MaxLength.Value] : value;
      _cursorPos = Math.Min(_cursorPos, field.Length);
      _scrollOffset = field.Length > 0 ? Math.Min(_scrollOffset, field.Length - 1) : 0;
      RequestRender();
    }
  } = "";
  /// <summary>
  /// Gets or sets the placeholder.
  /// </summary>
  public string? Placeholder { get; set; }
  /// <summary>
  /// Gets or sets the placeholder color.
  /// </summary>
  public string? PlaceholderColor { get; set; }
  /// <summary>
  /// Gets or sets the cursor color.
  /// </summary>
  public string? CursorColor { get; set; }
  /// <summary>
  /// Gets or sets the fg.
  /// </summary>
  public string? Fg { get; set; }
  /// <summary>
  /// Gets or sets the bg.
  /// </summary>
  public string? Bg { get; set; }
  /// <summary>
  /// Gets or sets the max length.
  /// </summary>
  public int? MaxLength { get; set; }

  private int _cursorPos = 0;
  private int _scrollOffset = 0;

  /// <summary>
  /// Input renderable.
  /// </summary>
  /// <param name="renderer">The renderer value.</param>
  /// <param name="options">The options value.</param>
  public InputRenderable(CliRenderer? renderer, InputOptions? options = null) : base(renderer)
  {
    var opts = options ?? new InputOptions();
    Placeholder = opts.Placeholder;
    PlaceholderColor = opts.PlaceholderColor;
    CursorColor = opts.CursorColor;
    Fg = opts.Fg;
    Bg = opts.Bg;
    MaxLength = opts.MaxLength;
    Focusable = true;
    if (opts.Width != null) SetInitialWidth(opts.Width);
    if (opts.Height != null) SetInitialHeight(opts.Height);
    if (opts.Value != null)
    {
      Value = opts.Value;
      _cursorPos = Value.Length;
    }

    On("focused", _ => RequestRender());
    On("blurred", _ =>
    {
      Emit("change", Value);
      RequestRender();
    });
  }

  /// <summary>
  /// Handle key.
  /// </summary>
  /// <param name="key">The key value.</param>
  public override void HandleKey(KeyEvent key)
  {
    switch (key.Name)
    {
      case "return":
        Emit("enter", Value);
        Emit("change", Value);
        RequestRender();
        break;
      case "backspace":
        if (_cursorPos > 0)
        {
          var newValue = Value[..(_cursorPos - 1)] + Value[_cursorPos..];
          _cursorPos--;
          Value = newValue;
          Emit("input", Value);
        }
        break;
      case "delete":
        if (_cursorPos < Value.Length)
        {
          Value = Value[.._cursorPos] + Value[(_cursorPos + 1)..];
          Emit("input", Value);
        }
        break;
      case "left":
        if (_cursorPos > 0) { _cursorPos--; RequestRender(); }
        break;
      case "right":
        if (_cursorPos < Value.Length) { _cursorPos++; RequestRender(); }
        break;
      case "home":
        _cursorPos = 0; RequestRender();
        break;
      case "end":
        _cursorPos = Value.Length; RequestRender();
        break;
      default:
        if (key.Char.HasValue && !key.Ctrl && !key.Alt)
        {
          if (MaxLength == null || Value.Length < MaxLength.Value)
          {
            var newValue = Value[.._cursorPos] + key.Char.Value + Value[_cursorPos..];
            _cursorPos++;
            Value = newValue;
            Emit("input", Value);
          }
        }
        break;
    }
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = Math.Max(1, ComputedHeight);
    if (w <= 0 || h <= 0) return;

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(255, 255, 255);
    var bg = Bg != null ? Rgba.FromCss(Bg) : Rgba.FromInts(30, 30, 30);

    buffer.FillRect(x, y, w, h, bg);

    if (Value.Length == 0 && Placeholder != null)
    {
      var phColor = PlaceholderColor != null ? Rgba.FromCss(PlaceholderColor) : Rgba.FromInts(100, 100, 100);
      var ph = Placeholder.Length > w ? Placeholder[..w] : Placeholder;
      buffer.DrawText(x, y, ph, phColor, bg);
    }
    else
    {
      // Scroll the view so cursor is visible
      if (_cursorPos - _scrollOffset >= w) _scrollOffset = _cursorPos - w + 1;
      if (_cursorPos < _scrollOffset) _scrollOffset = _cursorPos;

      var visible = Value.Length > _scrollOffset ? Value[_scrollOffset..] : "";
      if (visible.Length > w) visible = visible[..w];
      buffer.DrawText(x, y, visible, fg, bg);

      // Draw cursor
      if (Focused)
      {
        int cursorScreenX = x + _cursorPos - _scrollOffset;
        if (cursorScreenX >= x && cursorScreenX < x + w)
        {
          var cursorBg = CursorColor != null ? Rgba.FromCss(CursorColor) : Rgba.FromInts(255, 255, 255);
          char cursorChar = _cursorPos < Value.Length ? Value[_cursorPos] : ' ';
          buffer.SetCell(cursorScreenX, y, cursorChar, bg, cursorBg);
        }
      }
    }
  }
}
