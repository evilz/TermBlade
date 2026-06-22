using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Aligns child content inside its bounds.
/// </summary>
public sealed class Align : ContainerAdapterComponentBase<AlignRenderable>
{
  protected override AlignRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
