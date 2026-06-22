using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Compatibility alias for the misspelled dialog component name.
/// </summary>
public sealed class Dislog : ContainerAdapterComponentBase<DislogRenderable>
{
  protected override DislogRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
