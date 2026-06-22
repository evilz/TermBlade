using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a gradient brush preview.
/// </summary>
public sealed class GradientBrush : RenderableComponentBase<GradientBrushRenderable>
{
  [Parameter] public string StartColor { get; set; } = "#000000";
  [Parameter] public string EndColor { get; set; } = "#ffffff";

  protected override GradientBrushRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(GradientBrushRenderable renderable)
  {
    renderable.StartColor = StartColor;
    renderable.EndColor = EndColor;
    renderable.Fg = EndColor;
  }
}
