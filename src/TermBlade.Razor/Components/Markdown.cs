using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class Markdown : RenderableComponentBase<MarkdownRenderable>
{
  [Parameter] public string Content { get; set; } = string.Empty;

  protected override MarkdownRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(MarkdownRenderable renderable)
  {
    renderable.Content = Content;
  }
}
