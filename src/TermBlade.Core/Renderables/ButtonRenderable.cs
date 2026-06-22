using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders an interactive button label and emits a click event on Enter.
/// </summary>
public class ButtonRenderable : LabelRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ButtonRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ButtonRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
    Content = "[ Button ]";
  }

  /// <inheritdoc />
  public override void HandleKey(KeyEvent key)
  {
    if (key.Name is "return" or "space")
      Emit("clicked");
  }
}
