using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders image-like content on a terminal canvas.
/// </summary>
public class CanvasImageRenderable : FrameBufferRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="CanvasImageRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public CanvasImageRenderable(CliRenderer? renderer) : base(renderer, 80, 40)
  {
  }
}
