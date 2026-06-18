using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Console;

/// <summary>
/// Defines overlay position values.
/// </summary>
public enum OverlayPosition { TopLeft, TopRight, BottomLeft, BottomRight }

/// <summary>
/// Represents console overlay.
/// </summary>
public class ConsoleOverlay : TermBlade.Core.Renderables.Renderable
{
  /// <summary>
  /// Gets or sets the position.
  /// </summary>
  public new OverlayPosition Position { get; set; } = OverlayPosition.BottomRight;
  /// <summary>
  /// Gets or sets the max lines.
  /// </summary>
  public int MaxLines { get; set; } = 10;
  /// <summary>
  /// Gets or sets the max width.
  /// </summary>
  public int MaxWidth { get; set; } = 60;

  private readonly List<string> _lines = new();

  /// <summary>
  /// Console overlay.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public ConsoleOverlay(TermBlade.Core.Rendering.CliRenderer? renderer) : base(renderer)
  {
    LayoutNode.Position = "absolute";
  }

  /// <summary>
  /// Add line.
  /// </summary>
  /// <param name="line">The line value.</param>
  public void AddLine(string line)
  {
    _lines.Add(line);
    while (_lines.Count > MaxLines)
      _lines.RemoveAt(0);
    RequestRender();
  }

  /// <summary>
  /// Gets the log.
  /// </summary>
  public void Log(string message) => AddLine($"[LOG] {message}");
  /// <summary>
  /// Gets the warn.
  /// </summary>
  public void Warn(string message) => AddLine($"[WARN] {message}");
  /// <summary>
  /// Gets the error.
  /// </summary>
  public void Error(string message) => AddLine($"[ERR] {message}");

  /// <summary>
  /// Clear.
  /// </summary>
  public void Clear()
  {
    _lines.Clear();
    RequestRender();
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    if (_lines.Count == 0) return;

    int h = _lines.Count;
    int w = Math.Min(MaxWidth, _lines.Max(l => l.Length) + 2);
    int termW = buffer.Width;
    int termH = buffer.Height;

    int bx = Position switch
    {
      OverlayPosition.TopLeft or OverlayPosition.BottomLeft => 0,
      _ => termW - w
    };
    int by = Position switch
    {
      OverlayPosition.TopLeft or OverlayPosition.TopRight => 0,
      _ => termH - h
    };

    var bg = Rgba.FromInts(20, 20, 20);
    var fg = Rgba.FromInts(200, 200, 200);

    buffer.FillRect(bx, by, w, h, bg);
    for (int i = 0; i < h; i++)
    {
      var line = _lines[i];
      if (line.Length > w - 1) line = line[..(w - 1)];
      buffer.DrawText(bx + 1, by + i, line, fg, bg);
    }
  }
}
