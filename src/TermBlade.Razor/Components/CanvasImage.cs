using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders image-like content on a terminal canvas.
/// </summary>
public sealed class CanvasImage : RenderableComponentBase<CanvasImageRenderable>
{
  protected override CanvasImageRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
  protected override void ApplyParameters(CanvasImageRenderable renderable) { }
}
