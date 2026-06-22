using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders an icon glyph.
/// </summary>
public sealed class Icon : LabelRenderableComponentBase<IconRenderable>
{
  protected override IconRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
