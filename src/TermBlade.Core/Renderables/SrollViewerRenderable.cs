using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Compatibility alias for the misspelled scroll viewer name in older requests.
/// </summary>
public class SrollViewerRenderable : ScrollViewerRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SrollViewerRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public SrollViewerRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
