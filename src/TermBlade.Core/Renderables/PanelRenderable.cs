using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a bordered panel surface with optional title support.
/// </summary>
public class PanelRenderable : BoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PanelRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public PanelRenderable(CliRenderer? renderer) : base(renderer, new BoxOptions { Border = true, ShouldFill = true })
  {
  }
}
