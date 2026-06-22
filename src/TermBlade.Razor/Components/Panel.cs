using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a bordered panel container.
/// </summary>
public sealed class Panel : ContainerRenderableComponentBase<PanelRenderable>
{
  [Parameter] public string? Title { get; set; }
  [Parameter] public string BorderColor { get; set; } = "#ffffff";
  [Parameter] public string BackgroundColor { get; set; } = "transparent";

  protected override PanelRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(PanelRenderable renderable)
  {
    renderable.Title = Title;
    renderable.BorderColor = BorderColor;
    renderable.BackgroundColor = BackgroundColor;
  }
}
