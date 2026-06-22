using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Arranges children vertically in rows.
/// </summary>
public sealed class Rows : ContainerAdapterComponentBase<RowsRenderable>
{
  protected override RowsRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
