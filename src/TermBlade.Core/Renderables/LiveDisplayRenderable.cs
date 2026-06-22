using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a live-updating display container.
/// </summary>
public class LiveDisplayRenderable : SurfaceRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="LiveDisplayRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public LiveDisplayRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
