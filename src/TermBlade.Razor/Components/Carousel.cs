using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders the currently selected carousel item.
/// </summary>
public sealed class Carousel : RenderableComponentBase<CarouselRenderable>
{
  [Parameter] public IReadOnlyList<string> Items { get; set; } = [];
  [Parameter] public int SelectedIndex { get; set; }

  protected override CarouselRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(CarouselRenderable renderable)
  {
    renderable.Items = Items;
    renderable.SelectedIndex = SelectedIndex;
  }
}
