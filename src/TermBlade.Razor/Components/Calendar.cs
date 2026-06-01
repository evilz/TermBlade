using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// A monthly calendar widget. Displays the days of a given month in a 7-column grid,
/// with optional highlighting of a specific date and alternating row backgrounds.
/// </summary>
public sealed class Calendar : RenderableComponentBase<CalendarRenderable>
{
  [Parameter] public DateOnly DisplayMonth { get; set; } = DateOnly.FromDateTime(DateTime.Today);
  [Parameter] public DateOnly? HighlightedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
  [Parameter] public string TitleColor { get; set; } = "#ffffff";
  [Parameter] public string TitleAlignment { get; set; } = "center";
  [Parameter] public string DayNameColor { get; set; } = "#00c000";
  [Parameter] public string DayColor { get; set; } = "#ffffff";
  [Parameter] public string OtherMonthDayColor { get; set; } = "#555555";
  [Parameter] public string HighlightColor { get; set; } = "#ff0000";
  [Parameter] public string? BackgroundColor { get; set; }
  [Parameter] public string? AlternateRowBackgroundColor { get; set; }
  [Parameter] public bool ShowOtherMonthDays { get; set; } = true;

  protected override CalendarRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(CalendarRenderable renderable)
  {
    renderable.DisplayMonth = DisplayMonth;
    renderable.HighlightedDate = HighlightedDate;
    renderable.TitleColor = TitleColor;
    renderable.TitleAlignment = TitleAlignment;
    renderable.DayNameColor = DayNameColor;
    renderable.DayColor = DayColor;
    renderable.OtherMonthDayColor = OtherMonthDayColor;
    renderable.HighlightColor = HighlightColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.AlternateRowBackgroundColor = AlternateRowBackgroundColor;
    renderable.ShowOtherMonthDays = ShowOtherMonthDays;
  }
}
