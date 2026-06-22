using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Compatibility alias for the misspelled dialog name in older requests.
/// </summary>
public class DislogRenderable : DialogRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DislogRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public DislogRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
