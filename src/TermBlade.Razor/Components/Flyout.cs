using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a floating flyout panel.
/// </summary>
public sealed class Flyout : ContainerAdapterComponentBase<FlyoutRenderable>
{
  protected override FlyoutRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
