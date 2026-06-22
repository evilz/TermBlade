using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders Spectre-style markup text using TermBlade's markdown-compatible text surface.
/// </summary>
public class MarkupRenderable : MarkdownRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MarkupRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public MarkupRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
