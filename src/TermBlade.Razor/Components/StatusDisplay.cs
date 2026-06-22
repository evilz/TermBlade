using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a live status display.
/// </summary>
public sealed class StatusDisplay : LabelRenderableComponentBase<StatusDisplayRenderable>
{
  protected override StatusDisplayRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
