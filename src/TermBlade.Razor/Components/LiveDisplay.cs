using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a live display container.
/// </summary>
public sealed class LiveDisplay : ContainerAdapterComponentBase<LiveDisplayRenderable>
{
  protected override LiveDisplayRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
