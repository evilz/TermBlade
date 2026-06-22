using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders an expandable panel.
/// </summary>
public sealed class Expander : ContainerAdapterComponentBase<ExpanderRenderable>
{
  [Parameter] public bool IsExpanded { get; set; } = true;

  protected override ExpanderRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(ExpanderRenderable renderable) => renderable.IsExpanded = IsExpanded;
}
