using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Aligns child renderables inside a container.
/// </summary>
public class AlignRenderable : BoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AlignRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public AlignRenderable(CliRenderer? renderer) : base(renderer, new BoxOptions { AlignItems = AlignItems.Center, JustifyContent = JustifyContent.Center, ShouldFill = false })
  {
  }
}
