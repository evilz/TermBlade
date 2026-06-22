using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a filesystem or URI path.
/// </summary>
public sealed class TextPath : RenderableComponentBase<TextPathRenderable>
{
  [Parameter] public string Path { get; set; } = string.Empty;
  [Parameter] public string? Fg { get; set; }

  protected override TextPathRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(TextPathRenderable renderable)
  {
    renderable.Content = Path;
    renderable.Fg = Fg;
  }
}
