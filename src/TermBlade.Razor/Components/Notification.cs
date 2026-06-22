using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a notification message.
/// </summary>
public sealed class Notification : LabelRenderableComponentBase<NotificationRenderable>
{
  protected override NotificationRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
