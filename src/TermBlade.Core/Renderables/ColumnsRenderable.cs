using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Arranges child renderables in horizontal columns.
/// </summary>
public class ColumnsRenderable : BoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ColumnsRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ColumnsRenderable(CliRenderer? renderer) : base(renderer, new BoxOptions { FlexDirection = FlexDirection.Row, ShouldFill = false })
  {
  }
}
