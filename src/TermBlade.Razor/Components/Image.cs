using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders terminal image content.
/// </summary>
public sealed class Image : RenderableComponentBase<ImageRenderable>
{
  protected override ImageRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
  protected override void ApplyParameters(ImageRenderable renderable) { }
}
