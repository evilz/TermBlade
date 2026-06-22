using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a scrollable viewer container.
/// </summary>
public class ScrollViewerRenderable : ScrollBoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ScrollViewerRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ScrollViewerRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
