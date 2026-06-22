using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders an autocomplete prompt as a filtered selection list.
/// </summary>
public class AutoCompleteRenderable : SelectionPromptRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AutoCompleteRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public AutoCompleteRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
