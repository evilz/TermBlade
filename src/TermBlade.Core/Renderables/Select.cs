using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents select option.
/// </summary>
public class SelectOption
{
  /// <summary>
  /// Gets or sets the name.
  /// </summary>
  public string Name { get; set; } = "";
  /// <summary>
  /// Gets or sets the description.
  /// </summary>
  public string? Description { get; set; }
  /// <summary>
  /// Gets or sets the value.
  /// </summary>
  public object? Value { get; set; }
}

/// <summary>
/// Represents select renderable.
/// </summary>
public class SelectRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the options.
  /// </summary>
  public List<SelectOption> Options { get; set; } = new();
  /// <summary>
  /// Gets or sets the selected index.
  /// </summary>
  public int SelectedIndex { get; set; } = 0;
  /// <summary>
  /// Gets or sets the show description.
  /// </summary>
  public bool ShowDescription { get; set; } = true;
  /// <summary>
  /// Gets or sets the show scroll indicator.
  /// </summary>
  public bool ShowScrollIndicator { get; set; } = true;
  /// <summary>
  /// Gets or sets the selected bg.
  /// </summary>
  public string? SelectedBg { get; set; } = "#0055aa";
  /// <summary>
  /// Gets or sets the fg.
  /// </summary>
  public string? Fg { get; set; }

  private int _scrollOffset = 0;

  /// <summary>
  /// Select renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public SelectRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }

  /// <summary>
  /// Handle key.
  /// </summary>
  /// <param name="key">The key value.</param>
  public override void HandleKey(KeyEvent key)
  {
    switch (key.Name)
    {
      case "up":
        if (SelectedIndex > 0)
        {
          SelectedIndex--;
          EnsureVisible();
          Emit("selectionChanged", SelectedIndex);
          RequestRender();
        }
        break;
      case "down":
        if (SelectedIndex < Options.Count - 1)
        {
          SelectedIndex++;
          EnsureVisible();
          Emit("selectionChanged", SelectedIndex);
          RequestRender();
        }
        break;
      case "return":
        if (SelectedIndex < Options.Count)
          Emit("itemSelected", Options[SelectedIndex]);
        break;
    }
  }

  private void EnsureVisible()
  {
    int h = ComputedHeight;
    if (SelectedIndex < _scrollOffset) _scrollOffset = SelectedIndex;
    if (SelectedIndex >= _scrollOffset + h) _scrollOffset = SelectedIndex - h + 1;
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(255, 255, 255);
    var bg = Rgba.FromInts(0, 0, 0);
    var selBg = SelectedBg != null ? Rgba.FromCss(SelectedBg) : Rgba.FromInts(0, 85, 170);

    for (int row = 0; row < h; row++)
    {
      int idx = row + _scrollOffset;
      if (idx >= Options.Count) break;

      var opt = Options[idx];
      bool isSelected = idx == SelectedIndex;
      var rowBg = isSelected ? selBg : bg;

      buffer.FillRect(x, y + row, w, 1, rowBg);

      var name = opt.Name.Length > w - 2 ? opt.Name[..(w - 2)] : opt.Name;
      var prefix = isSelected ? "▶ " : "  ";
      buffer.DrawText(x, y + row, prefix + name, fg, rowBg);
    }

    if (ShowScrollIndicator && Options.Count > h)
    {
      int scrollX = x + w - 1;
      float ratio = (float)_scrollOffset / (Options.Count - h);
      int thumbY = (int)(ratio * (h - 1));
      for (int row = 0; row < h; row++)
      {
        var trackBg = Rgba.FromInts(40, 40, 40);
        buffer.SetCell(scrollX, y + row, '│', Rgba.FromInts(80, 80, 80), trackBg);
      }
      buffer.SetCell(scrollX, y + thumbY, '█', Rgba.FromInts(150, 150, 150), Rgba.FromInts(40, 40, 40));
    }
  }
}
