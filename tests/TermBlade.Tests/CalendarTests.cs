using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Tests;

public class CalendarTests
{
  // Render a CalendarRenderable into a buffer of the given size and return the buffer.
  private static RenderBuffer RenderCalendar(CalendarRenderable calendar, int width, int height)
  {
    // Wire up a root layout node so FlexLayout.Calculate populates ComputedWidth/Height.
    var root = new Core.Layout.FlexNode
    {
      FlexDirection = Core.Layout.FlexDirection.Column,
    };
    calendar.LayoutNode.Width = Core.Layout.LayoutDimension.Fixed(width);
    calendar.LayoutNode.Height = Core.Layout.LayoutDimension.Fixed(height);
    root.AddChild(calendar.LayoutNode);
    Core.Layout.FlexLayout.Calculate(root, width, height);

    var buffer = new RenderBuffer(width, height);
    calendar.Render(buffer, 0);
    return buffer;
  }

  // Helper: extract a row of text from the buffer as a string.
  private static string GetRow(RenderBuffer buffer, int row)
  {
    var chars = new System.Text.StringBuilder();
    for (int col = 0; col < buffer.Width; col++)
    {
      var cell = buffer.GetCell(col, row);
      chars.Append(cell.HasValue && cell.Value.Codepoint > 0
          ? char.ConvertFromUtf32(cell.Value.Codepoint)
          : " ");
    }
    return chars.ToString();
  }

  // Helper: get the foreground color at a specific cell.
  private static Rgba GetFg(RenderBuffer buffer, int col, int row)
      => buffer.GetCell(col, row)?.Fg ?? Rgba.FromInts(0, 0, 0);

  [Fact]
  public void TitleRow_ContainsMonthAndYear()
  {
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2007, 6, 1),
      HighlightedDate = null,
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    var title = GetRow(buffer, 0);

    Assert.Contains("June", title);
    Assert.Contains("2007", title);
  }

  [Fact]
  public void TitleRow_IsCenteredByDefault()
  {
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2025, 9, 1),
      HighlightedDate = null,
    };

    // "September 2025" = 14 chars; in 20-wide buffer centred → 3 leading spaces
    var buffer = RenderCalendar(calendar, 20, 8);
    var title = GetRow(buffer, 0);

    // Title should be centred, so there should be leading whitespace
    int firstChar = 0;
    while (firstChar < title.Length && title[firstChar] == ' ')
      firstChar++;
    Assert.True(firstChar > 0, $"Expected centred title but got: '{title}'");
  }

  [Fact]
  public void HeaderRow_ContainsAllDayAbbreviations()
  {
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2025, 9, 1),
      HighlightedDate = null,
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    var header = GetRow(buffer, 1);

    foreach (var day in new[] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" })
      Assert.Contains(day, header);
  }

  [Fact]
  public void September2025_StartsOnMonday_FirstDayCellIsColumn1()
  {
    // Sep 1 2025 is a Monday (DayOfWeek = 1), so column 0 (Sunday) should be blank
    // and "1" should appear in column 1.
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2025, 9, 1),
      HighlightedDate = null,
      ShowOtherMonthDays = false,
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    // Row 2 is the first date row. Column 0-1 (Sunday slot) should be blank, " 1" at column 3.
    var row = GetRow(buffer, 2);

    // The first character of row 2 should be a space (Sunday cell is empty).
    Assert.Equal(' ', row[0]);
    // " 1" begins at col index 3 (Monday = col 1, cellX = 1 * 3 = 3).
    Assert.Equal(" 1", row.Substring(3, 2));
  }

  [Fact]
  public void June2007_StartsOnFriday_PreviousMonthDaysVisible()
  {
    // June 1 2007 is a Friday (DayOfWeek = 5).
    // With ShowOtherMonthDays=true, Sunday-Thursday of that first week (May 27-31) should appear.
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2007, 6, 1),
      HighlightedDate = null,
      ShowOtherMonthDays = true,
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    var row = GetRow(buffer, 2); // first date row

    // "27" should appear at Sunday slot (col 0, cellX=0)
    Assert.Equal("27", row.Substring(0, 2));
  }

  [Fact]
  public void June2007_StartsOnFriday_PreviousMonthDaysHidden()
  {
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2007, 6, 1),
      HighlightedDate = null,
      ShowOtherMonthDays = false,
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    var row = GetRow(buffer, 2); // first date row

    // With ShowOtherMonthDays=false, Sunday slot should be blank.
    Assert.Equal(' ', row[0]);
    Assert.Equal(' ', row[1]);
  }

  [Fact]
  public void HighlightedDate_UsesHighlightColor()
  {
    var highlightColor = "#ff0000";
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2025, 9, 1),
      HighlightedDate = new DateOnly(2025, 9, 1), // Monday = col 1
      HighlightColor = highlightColor,
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    // Sep 1 is Monday (col 1), row 2. cellX = 1*3 = 3.
    var fg = GetFg(buffer, 3, 2);
    var expected = Rgba.FromCss(highlightColor);

    Assert.Equal(expected.R, fg.R);
    Assert.Equal(expected.G, fg.G);
    Assert.Equal(expected.B, fg.B);
  }

  [Fact]
  public void OtherMonthDays_UseOtherMonthColor()
  {
    var otherColor = "#555555";
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2007, 6, 1),
      HighlightedDate = null,
      ShowOtherMonthDays = true,
      OtherMonthDayColor = otherColor,
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    // May 27 is at col 0, row 2
    var fg = GetFg(buffer, 0, 2);
    var expected = Rgba.FromCss(otherColor);

    Assert.Equal(expected.R, fg.R);
    Assert.Equal(expected.G, fg.G);
    Assert.Equal(expected.B, fg.B);
  }

  [Fact]
  public void TitleAlignment_Right_TitleIsRightAligned()
  {
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2007, 6, 1),
      HighlightedDate = null,
      TitleAlignment = "right",
    };

    var buffer = RenderCalendar(calendar, 20, 8);
    var title = GetRow(buffer, 0);

    // "June 2007" = 9 chars; right-aligned in 20 → should end at position 19
    int lastChar = title.TrimEnd().Length;
    // Allow 1 char of slack due to centering rounding
    Assert.True(lastChar >= 19, $"Expected right-aligned title but got: '{title}'");
  }

  [Fact]
  public void AllDatesInMonthAreRendered()
  {
    // February 2024 (leap year, 29 days, starts Thursday)
    var calendar = new CalendarRenderable(null)
    {
      DisplayMonth = new DateOnly(2024, 2, 1),
      HighlightedDate = null,
    };

    var buffer = RenderCalendar(calendar, 20, 9);

    // All 29 days should appear somewhere in the buffer
    for (int day = 1; day <= 29; day++)
    {
      var dayStr = day.ToString().PadLeft(2);
      bool found = false;
      for (int row = 2; row < buffer.Height && !found; row++)
        if (GetRow(buffer, row).Contains(dayStr))
          found = true;
      Assert.True(found, $"Day {day} not found in calendar output");
    }
  }
}
