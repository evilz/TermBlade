using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a relative-positioned panel.
/// </summary>
public sealed class RelativePanel : ContainerAdapterComponentBase<RelativePanelRenderable>
{
  protected override RelativePanelRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
