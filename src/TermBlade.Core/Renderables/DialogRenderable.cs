using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a bordered dialog container.
/// </summary>
public class DialogRenderable : PanelRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DialogRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public DialogRenderable(CliRenderer? renderer) : base(renderer)
  {
    Title = "Dialog";
  }
}
