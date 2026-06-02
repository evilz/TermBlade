using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class Spinner : RenderableComponentBase<SpinnerRenderable>
{
  [Parameter] public string Title { get; set; } = "";
  [Parameter] public string[]? Frames { get; set; }
  [Parameter] public double Interval { get; set; } = 0.08;
  [Parameter] public string? Fg { get; set; }
  [Parameter] public string? Bg { get; set; }
  [Parameter] public string? SpinnerColor { get; set; }
  [Parameter] public bool IsSpinning { get; set; } = true;
  [Parameter] public string? CompletedText { get; set; }

  protected override SpinnerRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(SpinnerRenderable renderable)
  {
    renderable.Title = Title;
    if (Frames != null)
      renderable.Frames = Frames;
    renderable.Interval = Interval;
    renderable.Fg = Fg;
    renderable.Bg = Bg;
    renderable.SpinnerColor = SpinnerColor;
    renderable.IsSpinning = IsSpinning;
    renderable.CompletedText = CompletedText;
  }
}
