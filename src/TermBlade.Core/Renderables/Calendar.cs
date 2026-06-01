using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// A monthly calendar widget that displays a grid of days for a given month,
/// with optional highlighting of a specific date and alternating row backgrounds.
/// </summary>
public class CalendarRenderable : Renderable
{
  private static readonly string[] DayHeaders = ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"];

  /// <summary>The month to display. Defaults to the current month.</summary>
  public DateOnly DisplayMonth { get; set; } = DateOnly.FromDateTime(DateTime.Today);

  /// <summary>The date to highlight (e.g. today or a selected date). Null disables highlighting.</summary>
  public DateOnly? HighlightedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

  /// <summary>CSS color for the month/year title. Defaults to white.</summary>
  public string TitleColor { get; set; } = "#ffffff";

  /// <summary>Alignment of the month/year title: "left", "center", or "right". Defaults to "center".</summary>
  public string TitleAlignment { get; set; } = "center";

  /// <summary>CSS color for the day-of-week header row. Defaults to bright green.</summary>
  public string DayNameColor { get; set; } = "#00c000";

  /// <summary>CSS color for normal days in the display month. Defaults to white.</summary>
  public string DayColor { get; set; } = "#ffffff";

  /// <summary>CSS color for days that belong to the previous or next month. Defaults to dim gray.</summary>
  public string OtherMonthDayColor { get; set; } = "#555555";

  /// <summary>CSS color used to render the highlighted date. Defaults to red.</summary>
  public string HighlightColor { get; set; } = "#ff0000";

  /// <summary>Optional CSS background color for the whole widget.</summary>
  public string? BackgroundColor { get; set; }

  /// <summary>
  /// Optional CSS color used for every other calendar row (0-indexed: rows 1, 3, 5).
  /// When null (the default) row backgrounds are uniform.
  /// </summary>
  public string? AlternateRowBackgroundColor { get; set; }

  /// <summary>
  /// When true, days from the previous/next month are rendered in OtherMonthDayColor
  /// to fill out the first and last weeks. When false those cells are left blank.
  /// Defaults to true.
  /// </summary>
  public bool ShowOtherMonthDays { get; set; } = true;

  public CalendarRenderable(CliRenderer? renderer) : base(renderer) { }

  // The calendar content is always 20 characters wide:
  // 7 columns × 3 chars (2-digit day + 1 separator) minus the trailing separator = 20.
  private const int GridWidth = 20;

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);
    var altBg = AlternateRowBackgroundColor != null ? Rgba.FromCss(AlternateRowBackgroundColor) : bg;
    var titleFg = Rgba.FromCss(TitleColor);
    var dayNameFg = Rgba.FromCss(DayNameColor);
    var dayFg = Rgba.FromCss(DayColor);
    var otherFg = Rgba.FromCss(OtherMonthDayColor);
    var highlightFg = Rgba.FromCss(HighlightColor);

    // Background fill
    if (BackgroundColor != null && bg.AlphaByte > 0)
      buffer.FillRect(x, y, w, h, bg);

    // ── Row 0: Month/Year title ──────────────────────────────────────────────
    if (h > 0)
    {
      var title = DisplayMonth.ToString("MMMM yyyy");
      int titleX = TitleAlignment switch
      {
        "right" => x + Math.Max(0, GridWidth - title.Length),
        "left" => x,
        _ => x + Math.Max(0, (GridWidth - title.Length) / 2), // "center"
      };
      buffer.DrawText(titleX, y, title, titleFg, bg, TextAttributes.Bold);
    }

    // ── Row 1: Day-of-week header ────────────────────────────────────────────
    if (h > 1)
    {
      for (int col = 0; col < 7; col++)
      {
        int cellX = x + col * 3;
        buffer.DrawText(cellX, y + 1, DayHeaders[col], dayNameFg, bg);
      }
    }

    // ── Rows 2+: Calendar grid ───────────────────────────────────────────────
    var firstDay = new DateOnly(DisplayMonth.Year, DisplayMonth.Month, 1);
    int startDow = (int)firstDay.DayOfWeek; // 0 = Sunday
    var gridStart = firstDay.AddDays(-startDow);

    int daysInMonth = DateTime.DaysInMonth(DisplayMonth.Year, DisplayMonth.Month);
    int totalCells = startDow + daysInMonth;
    int rows = (totalCells + 6) / 7; // ceiling division

    for (int row = 0; row < rows; row++)
    {
      int rowY = y + 2 + row;
      if (rowY >= y + h) break;

      // Alternating row background
      var rowBg = (row % 2 == 1 && AlternateRowBackgroundColor != null) ? altBg : bg;
      if (AlternateRowBackgroundColor != null && row % 2 == 1)
        buffer.FillRect(x, rowY, Math.Min(GridWidth, w), 1, rowBg);

      for (int col = 0; col < 7; col++)
      {
        var date = gridStart.AddDays(row * 7 + col);
        bool isCurrentMonth = date.Month == DisplayMonth.Month && date.Year == DisplayMonth.Year;

        if (!isCurrentMonth && !ShowOtherMonthDays)
          continue;

        int cellX = x + col * 3;
        if (cellX + 2 > x + w) break;

        var dayStr = date.Day.ToString().PadLeft(2);

        Rgba fg;
        if (HighlightedDate.HasValue && date == HighlightedDate.Value)
          fg = highlightFg;
        else if (!isCurrentMonth)
          fg = otherFg;
        else
          fg = dayFg;

        buffer.DrawText(cellX, rowY, dayStr, fg, rowBg);
      }
    }
  }
}
