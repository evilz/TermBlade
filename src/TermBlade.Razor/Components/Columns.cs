using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Arranges children horizontally in columns.
/// </summary>
public sealed class Columns : ContainerAdapterComponentBase<ColumnsRenderable>
{
  protected override ColumnsRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
