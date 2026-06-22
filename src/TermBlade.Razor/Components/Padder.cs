using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Adds padding around child content.
/// </summary>
public sealed class Padder : ContainerAdapterComponentBase<PadderRenderable>
{
  protected override PadderRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
