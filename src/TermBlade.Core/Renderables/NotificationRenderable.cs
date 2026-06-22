using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a notification message.
/// </summary>
public class NotificationRenderable : LabelRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="NotificationRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public NotificationRenderable(CliRenderer? renderer) : base(renderer)
  {
    Attributes = TextAttributes.Bold;
  }
}
