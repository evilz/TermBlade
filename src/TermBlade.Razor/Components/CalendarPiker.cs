using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Compatibility alias for the misspelled calendar picker component name.
/// </summary>
public sealed class CalendarPiker : RenderableComponentBase<CalendarPikerRenderable>
{
  protected override CalendarPikerRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
  protected override void ApplyParameters(CalendarPikerRenderable renderable) { }
}
