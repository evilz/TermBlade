using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a modal message box.
/// </summary>
public class MessageBoxRenderable : DialogRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MessageBoxRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public MessageBoxRenderable(CliRenderer? renderer) : base(renderer)
  {
    Title = "Message";
  }
}
