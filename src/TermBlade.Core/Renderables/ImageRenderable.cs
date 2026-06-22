using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders terminal image content through a frame buffer.
/// </summary>
public class ImageRenderable : CanvasImageRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ImageRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ImageRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
