using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a filesystem or URI path as text.
/// </summary>
public class TextPathRenderable : TextRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="TextPathRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public TextPathRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
