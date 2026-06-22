using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Arranges child renderables in vertical rows.
/// </summary>
public class RowsRenderable : BoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RowsRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public RowsRenderable(CliRenderer? renderer) : base(renderer, new BoxOptions { FlexDirection = FlexDirection.Column, ShouldFill = false })
  {
  }
}
