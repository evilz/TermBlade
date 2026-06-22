using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders Spectre-style markup content.
/// </summary>
public sealed class Markup : RenderableComponentBase<MarkupRenderable>
{
  [Parameter] public string Content { get; set; } = string.Empty;

  protected override MarkupRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(MarkupRenderable renderable) => renderable.Content = Content;
}
