using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a multi-selection prompt.
/// </summary>
public class MultiSelectionPromptRenderable : MultiSelectRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MultiSelectionPromptRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public MultiSelectionPromptRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
