using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders an icon glyph.
/// </summary>
public class IconRenderable : LabelRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="IconRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public IconRenderable(CliRenderer? renderer) : base(renderer)
  {
    Content = "*";
  }
}
