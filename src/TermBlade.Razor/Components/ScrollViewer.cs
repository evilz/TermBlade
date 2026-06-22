using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a scrollable viewer.
/// </summary>
public sealed class ScrollViewer : ContainerAdapterComponentBase<ScrollViewerRenderable>
{
  protected override ScrollViewerRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
