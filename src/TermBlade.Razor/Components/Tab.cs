using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a tab item.
/// </summary>
public sealed class Tab : RenderableComponentBase<TabRenderable>
{
  [Parameter] public string Content { get; set; } = string.Empty;
  [Parameter] public bool IsSelected { get; set; }

  protected override TabRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(TabRenderable renderable)
  {
    renderable.Content = Content;
    renderable.IsSelected = IsSelected;
  }
}
