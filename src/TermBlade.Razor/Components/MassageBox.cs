using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Compatibility alias for the misspelled message box component name.
/// </summary>
public sealed class MassageBox : ContainerAdapterComponentBase<MassageBoxRenderable>
{
  protected override MassageBoxRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
