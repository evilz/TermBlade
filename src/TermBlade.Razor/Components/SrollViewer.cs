using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Compatibility alias for the misspelled scroll viewer component name.
/// </summary>
public sealed class SrollViewer : ContainerAdapterComponentBase<SrollViewerRenderable>
{
  protected override SrollViewerRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
