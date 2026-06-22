using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders tabular data as a data grid.
/// </summary>
public sealed class DataGrid : RenderableComponentBase<DataGridRenderable>
{
  [Parameter] public IReadOnlyList<string> Columns { get; set; } = [];
  [Parameter] public IReadOnlyList<IReadOnlyList<string>> Rows { get; set; } = [];

  protected override DataGridRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(DataGridRenderable renderable)
  {
    renderable.Columns = Columns;
    renderable.Rows = Rows;
  }
}
