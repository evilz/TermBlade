using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Provides a reusable bordered container renderable for panel-like widgets.
/// </summary>
public class SurfaceRenderable : BoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SurfaceRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public SurfaceRenderable(CliRenderer? renderer) : base(renderer, new BoxOptions { Border = true, ShouldFill = true })
  {
  }
}
