using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a simple terminal drawing canvas.
/// </summary>
public class CanvasRenderable : FrameBufferRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="CanvasRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public CanvasRenderable(CliRenderer? renderer) : base(renderer, 80, 40)
  {
  }
}
