using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Provides a layout region container.
/// </summary>
public sealed class Layout : ContainerAdapterComponentBase<LayoutRenderable>
{
  protected override LayoutRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
