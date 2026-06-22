using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a combobox prompt as editable text with selectable options.
/// </summary>
public class ComboBoxRenderable : SelectionPromptRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ComboBoxRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ComboBoxRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
