using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders tabular data as a data grid.
/// </summary>
public class DataGridRenderable : TableRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DataGridRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public DataGridRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
