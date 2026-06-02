using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
using TermBlade.Core.Input;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

public class MultiSelectRenderable : Renderable
{
  public List<SelectOption> Options { get; set; } = new();
  public HashSet<int> SelectedIndices { get; set; } = new();
  public int CursorIndex { get; set; }
  public bool ShowDescription { get; set; } = true;
  public bool ShowScrollIndicator { get; set; } = true;
  public string? CursorBg { get; set; } = "#333333";
  public string? SelectedBg { get; set; } = "#0055aa";
  public string? Fg { get; set; }

  private int _scrollOffset;

  public MultiSelectRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }

  public override void HandleKey(KeyEvent key)
  {
    switch (key.Name)
    {
      case "up":
        if (CursorIndex > 0)
        {
          CursorIndex--;
          EnsureVisible();
          Emit("cursorChanged", CursorIndex);
          RequestRender();
        }
        break;
      case "down":
        if (CursorIndex < Options.Count - 1)
        {
          CursorIndex++;
          EnsureVisible();
          Emit("cursorChanged", CursorIndex);
          RequestRender();
        }
        break;
      case "space":
        if (CursorIndex >= 0 && CursorIndex < Options.Count)
        {
          if (!SelectedIndices.Remove(CursorIndex))
            SelectedIndices.Add(CursorIndex);
          Emit("selectionChanged", SelectedIndices.OrderBy(i => i).ToList());
          RequestRender();
        }
        break;
      case "return":
        Emit("confirmed", SelectedIndices.OrderBy(i => i).ToList());
        break;
    }
  }

  private void EnsureVisible()
  {
    int h = ComputedHeight;
    if (CursorIndex < _scrollOffset) _scrollOffset = CursorIndex;
    if (CursorIndex >= _scrollOffset + h) _scrollOffset = CursorIndex - h + 1;
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;
    NormalizeState(h);

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(255, 255, 255);
    var bg = Rgba.FromInts(0, 0, 0);
    var cursorBg = CursorBg != null ? Rgba.FromCss(CursorBg) : Rgba.FromInts(51, 51, 51);
    var selBg = SelectedBg != null ? Rgba.FromCss(SelectedBg) : Rgba.FromInts(0, 85, 170);

    for (int row = 0; row < h; row++)
    {
      int idx = row + _scrollOffset;
      if (idx >= Options.Count) break;

      var opt = Options[idx];
      bool isCursor = idx == CursorIndex;
      bool isSelected = SelectedIndices.Contains(idx);
      var rowBg = isCursor ? cursorBg : bg;
      if (isSelected && !isCursor) rowBg = selBg;

      buffer.FillRect(x, y + row, w, 1, rowBg);

      var check = isSelected ? "[x] " : "[ ] ";
      var text = check + opt.Name;
      if (ShowDescription && !string.IsNullOrEmpty(opt.Description))
        text += $" - {opt.Description}";
      text = TruncateToCellWidth(text, w);
      buffer.DrawText(x, y + row, text, fg, rowBg);
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

  private void NormalizeState(int height)
  {
    if (Options.Count == 0)
    {
      CursorIndex = 0;
      _scrollOffset = 0;
      SelectedIndices.RemoveWhere(i => i < 0);
      return;
    }

    CursorIndex = Math.Clamp(CursorIndex, 0, Options.Count - 1);
    _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, Options.Count - height));
    EnsureVisible();
    SelectedIndices.RemoveWhere(i => i < 0 || i >= Options.Count);
  }

  private static string TruncateToCellWidth(string text, int maxWidth)
  {
    if (maxWidth <= 0 || string.IsNullOrEmpty(text)) return string.Empty;

    int width = 0;
    int length = 0;
    foreach (var rune in text.EnumerateRunes())
    {
      int runeWidth = CellBuffer.RuneWidth(rune);
      if (width + runeWidth > maxWidth) break;
      width += runeWidth;
      length += rune.Utf16SequenceLength;
    }

    return text.Length == length ? text : text[..length];
  }
}
