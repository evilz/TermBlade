using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a text prompt based on the input control.
/// </summary>
public class TextPromptRenderable : InputRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="TextPromptRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public TextPromptRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
