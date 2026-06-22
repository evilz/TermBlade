using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a modal dialog container.
/// </summary>
public sealed class Dialog : ContainerAdapterComponentBase<DialogRenderable>
{
  protected override DialogRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
