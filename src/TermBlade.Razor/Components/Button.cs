using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a focusable button.
/// </summary>
public sealed class Button : LabelRenderableComponentBase<ButtonRenderable>
{
  [Parameter] public EventCallback OnClick { get; set; }

  protected override ButtonRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(ButtonRenderable renderable)
  {
    renderable.On("clicked", _ => DispatchEvent(OnClick.InvokeAsync));
  }
}
