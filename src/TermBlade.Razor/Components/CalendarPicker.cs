using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a calendar picker.
/// </summary>
public sealed class CalendarPicker : RenderableComponentBase<CalendarPickerRenderable>
{
  [Parameter] public DateOnly DisplayMonth { get; set; } = DateOnly.FromDateTime(DateTime.Today);
  [Parameter] public DateOnly? HighlightedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

  protected override CalendarPickerRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(CalendarPickerRenderable renderable)
  {
    renderable.DisplayMonth = DisplayMonth;
    renderable.HighlightedDate = HighlightedDate;
  }
}
