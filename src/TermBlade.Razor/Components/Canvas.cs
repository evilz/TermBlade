using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a terminal drawing canvas.
/// </summary>
public sealed class Canvas : RenderableComponentBase<CanvasRenderable>
{
  protected override CanvasRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
  protected override void ApplyParameters(CanvasRenderable renderable) { }
}
