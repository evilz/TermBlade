using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a single-selection prompt.
/// </summary>
public class SelectionPromptRenderable : SelectRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SelectionPromptRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public SelectionPromptRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
