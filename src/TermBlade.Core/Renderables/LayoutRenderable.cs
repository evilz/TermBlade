using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Provides a named layout container for composing terminal regions.
/// </summary>
public class LayoutRenderable : BoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="LayoutRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public LayoutRenderable(CliRenderer? renderer) : base(renderer, new BoxOptions { FlexGrow = 1, ShouldFill = false })
  {
  }
}
