using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Arranges children in a grid-like container.
/// </summary>
public sealed class Grid : ContainerAdapterComponentBase<GridRenderable>
{
  protected override GridRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
