using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

public class SpinnerRenderable : Renderable
{
  private static readonly string[] DefaultFrames = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];

  public string Title { get; set; } = "";
  public string[] Frames { get; set; } = DefaultFrames;
  public double Interval { get; set; } = 0.08;
  public string? Fg { get; set; }
  public string? Bg { get; set; }
  public string? SpinnerColor { get; set; }
  public bool IsSpinning { get; set; } = true;
  public string? CompletedText { get; set; }

  private double _elapsed;
  private int _frameIndex;

  public SpinnerRenderable(CliRenderer? renderer) : base(renderer)
  {
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(255, 255, 255);
    var bg = Bg != null ? Rgba.FromCss(Bg) : Rgba.FromInts(0, 0, 0);
    var spinnerFg = SpinnerColor != null ? Rgba.FromCss(SpinnerColor) : Rgba.FromInts(88, 166, 255);

    buffer.FillRect(x, y, w, h, bg);

    if (!IsSpinning)
    {
      var done = CompletedText ?? $"✔ {Title}";
      if (done.Length > w) done = done[..w];
      buffer.DrawText(x, y, done, fg, bg);
      return;
    }

    if (Frames.Length == 0) return;

    _elapsed += deltaTime;
    if (_elapsed >= Interval)
    {
      _elapsed -= Interval;
      _frameIndex = (_frameIndex + 1) % Frames.Length;
    }

    var frame = Frames[_frameIndex];
    buffer.DrawText(x, y, frame, spinnerFg, bg);

    var title = Title;
    var titleX = x + frame.Length + 1;
    var maxLen = w - frame.Length - 1;
    if (maxLen > 0 && title.Length > 0)
    {
      if (title.Length > maxLen) title = title[..maxLen];
      buffer.DrawText(titleX, y, title, fg, bg);
    }

    RequestRender();
  }
}
