using System.Text;
using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Docs.Wasm.Terminal.Previews;

/// <summary>
/// Builds the component preview frames rendered by the documentation WebAssembly app.
/// The previews are drawn through TermBlade's cell-based <see cref="RenderBuffer"/>
/// so xterm.js receives ANSI generated from the same primitives used by terminal UIs.
/// </summary>
public static class ComponentPreviewService
{
  private const int Width = 54;
  private const int Height = 12;
  private static readonly Rgba DefaultFg = Rgba.FromCss("#c0caf5");
  private static readonly Rgba MutedFg = Rgba.FromCss("#6e7681");
  private static readonly Rgba AccentFg = Rgba.FromCss("#3fb950");
  private static readonly Rgba BlueFg = Rgba.FromCss("#7aa2f7");
  private static readonly Rgba YellowFg = Rgba.FromCss("#e0af68");
  private static readonly Rgba RedFg = Rgba.FromCss("#f7768e");
  private static readonly Rgba MagentaFg = Rgba.FromCss("#bb9af7");
  private static readonly Rgba CyanFg = Rgba.FromCss("#7dcfff");
  private static readonly Rgba Bg = Rgba.FromCss("#1a1b26");
  private static readonly Rgba PanelBg = Rgba.FromCss("#161b22");
  private static readonly Rgba CursorBg = Rgba.FromCss("#33467c");
  private static readonly Rgba SelectedBg = Rgba.FromCss("#0055aa");

  public static string RenderPreview(string component)
  {
    var buffer = new RenderBuffer(Width, Height);
    buffer.Clear(Bg);

    switch (component.ToLowerInvariant())
    {
      case "text": RenderText(buffer); break;
      case "code": RenderCode(buffer); break;
      case "markdown": RenderMarkdown(buffer); break;
      case "diff": RenderDiff(buffer); break;
      case "asciifont": RenderAsciiFont(buffer); break;
      case "linenumbers": RenderLineNumbers(buffer); break;
      case "framebuffer": RenderFrameBuffer(buffer); break;
      case "calendar": RenderCalendar(buffer); break;
      case "input": RenderInput(buffer); break;
      case "textarea": RenderTextarea(buffer); break;
      case "slider": RenderSlider(buffer); break;
      case "select": RenderSelect(buffer); break;
      case "treeview": RenderTreeView(buffer); break;
      case "tabselect": RenderTabSelect(buffer); break;
      case "box": RenderBox(buffer); break;
      case "scrollbox": RenderScrollBox(buffer); break;
      case "scrollbar": RenderScrollBar(buffer); break;
      case "consoleoverlay": RenderConsoleOverlay(buffer); break;
      case "barchart": RenderBarChart(buffer); break;
      case "linechart": RenderLineChart(buffer); break;
      case "timeserieslinechart": RenderTimeSeriesLineChart(buffer); break;
      case "heatmap": RenderHeatMap(buffer); break;
      case "candlestickchart": RenderCandlestickChart(buffer); break;
      case "piechart": RenderPieChart(buffer); break;
      case "doughnutchart": RenderDoughnutChart(buffer); break;
      case "confirm": RenderConfirm(buffer); break;
      case "spinner": RenderSpinner(buffer); break;
      case "multiselect": RenderMultiSelect(buffer); break;
      default: RenderUnknown(buffer, component); break;
    }

    return ToAnsi(buffer);
  }

  private static void RenderText(RenderBuffer b)
  {
    Draw(b, 2, 1, "TermBlade Text", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, "Normal text", DefaultFg);
    Draw(b, 2, 4, "Bold text", DefaultFg, TextAttributes.Bold);
    Draw(b, 2, 5, "Italic text", DefaultFg, TextAttributes.Italic);
    Draw(b, 2, 6, "Underline text", DefaultFg, TextAttributes.Underline);
    Draw(b, 2, 7, "Strikethrough", DefaultFg, TextAttributes.Strikethrough);
    Draw(b, 2, 9, "RGB foreground + transparent background", AccentFg);
  }

  private static void RenderCode(RenderBuffer b)
  {
    Draw(b, 2, 1, "C#", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, "public", MagentaFg, TextAttributes.Bold);
    Draw(b, 9, 3, "void", BlueFg);
    Draw(b, 14, 3, "Render()", AccentFg);
    Draw(b, 23, 3, " {", DefaultFg);
    Draw(b, 4, 4, "buffer.DrawText(0, 0, \"Hello\");", DefaultFg);
    Draw(b, 2, 5, "}", DefaultFg);
    Draw(b, 2, 8, "Syntax highlighting from TermBlade Code", MutedFg);
  }

  private static void RenderMarkdown(RenderBuffer b)
  {
    Draw(b, 2, 1, "Markdown", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, "# Title", AccentFg, TextAttributes.Bold);
    Draw(b, 2, 4, "• list item", DefaultFg);
    Draw(b, 2, 5, "• another item", DefaultFg);
    Draw(b, 2, 7, "`inline code`", BlueFg);
    Draw(b, 2, 9, "Terminal-native markdown rendering", MutedFg);
  }

  private static void RenderDiff(RenderBuffer b)
  {
    Draw(b, 2, 1, "Diff", YellowFg, TextAttributes.Bold);
    Fill(b, 2, 3, 18, Rgba.FromCss("#3b1d24"));
    Draw(b, 2, 3, "- old line", RedFg);
    Fill(b, 2, 4, 18, Rgba.FromCss("#173923"));
    Draw(b, 2, 4, "+ new line", AccentFg);
    Draw(b, 2, 6, "  unchanged context", MutedFg);
    Draw(b, 2, 8, "Inline additions and removals", DefaultFg);
  }

  private static void RenderAsciiFont(RenderBuffer b)
  {
    Draw(b, 2, 1, "AsciiFont", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, "█████ █████ █████", AccentFg, TextAttributes.Bold);
    Draw(b, 2, 4, "  █   █     █  █ ", AccentFg, TextAttributes.Bold);
    Draw(b, 2, 5, "  █   ███   ███  ", AccentFg, TextAttributes.Bold);
    Draw(b, 2, 6, "  █   █     █  █ ", AccentFg, TextAttributes.Bold);
    Draw(b, 2, 7, "  █   █████ █  █ ", AccentFg, TextAttributes.Bold);
  }

  private static void RenderLineNumbers(RenderBuffer b)
  {
    Draw(b, 2, 1, "LineNumbers", YellowFg, TextAttributes.Bold);
    for (var i = 0; i < 5; i++)
    {
      Draw(b, 2, 3 + i, (i + 1).ToString().PadLeft(3), MutedFg);
      Draw(b, 7, 3 + i, i == 2 ? "current line" : "content", i == 2 ? AccentFg : DefaultFg);
    }
  }

  private static void RenderFrameBuffer(RenderBuffer b)
  {
    DrawBox(b, 2, 1, 31, 8, "FrameBuffer", CyanFg, "rounded");
    Draw(b, 5, 3, "┌───┐", AccentFg);
    Draw(b, 5, 4, "│ ▣ │  composable cell grid", AccentFg);
    Draw(b, 5, 5, "└───┘", AccentFg);
    Draw(b, 5, 7, "blit + alpha blend", MutedFg);
  }

  private static void RenderCalendar(RenderBuffer b)
  {
    Draw(b, 2, 1, "Calendar - June 2026", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, "Mo Tu We Th Fr Sa Su", CyanFg);
    Draw(b, 2, 4, " 1  2  3  4  5  6  7", DefaultFg);
    Draw(b, 2, 5, " 8  9 10 11 12 13 14", DefaultFg);
    Draw(b, 2, 6, "15 16 17 18 19 20 21", DefaultFg);
    Draw(b, 2, 7, "22 23 24 25 26 27 28", DefaultFg);
    Draw(b, 2, 8, "29 30", DefaultFg);
    Draw(b, 8, 5, "10", Bg, TextAttributes.Bold, SelectedBg);
  }

  private static void RenderInput(RenderBuffer b)
  {
    Draw(b, 2, 1, "Input", YellowFg, TextAttributes.Bold);
    DrawBox(b, 2, 3, 34, 3, null, BlueFg, "single");
    Draw(b, 4, 4, "termblade@example.com", DefaultFg);
    Draw(b, 25, 4, " ", DefaultFg, TextAttributes.None, CursorBg);
    Draw(b, 2, 7, "Single-line editable value", MutedFg);
  }

  private static void RenderTextarea(RenderBuffer b)
  {
    Draw(b, 2, 1, "Textarea", YellowFg, TextAttributes.Bold);
    DrawBox(b, 2, 3, 38, 6, null, BlueFg, "single");
    Draw(b, 4, 4, "Multi-line", DefaultFg);
    Draw(b, 4, 5, "editable content", DefaultFg);
    Draw(b, 4, 6, "with cursor", DefaultFg);
    Draw(b, 15, 6, " ", DefaultFg, TextAttributes.None, CursorBg);
  }

  private static void RenderSlider(RenderBuffer b)
  {
    Draw(b, 2, 1, "Slider", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 4, "Volume", DefaultFg);
    Draw(b, 11, 4, "[████████░░░░░░░] 53%", AccentFg);
    Draw(b, 11, 6, "0        50       100", MutedFg);
  }

  private static void RenderSelect(RenderBuffer b)
  {
    Draw(b, 2, 1, "Select", YellowFg, TextAttributes.Bold);
    var items = new[] { "Dashboard", "Components", "Settings", "About" };
    for (var i = 0; i < items.Length; i++)
    {
      if (i == 1) Fill(b, 2, 3 + i, 24, CursorBg);
      Draw(b, 4, 3 + i, (i == 1 ? "▶ " : "  ") + items[i], i == 1 ? DefaultFg : MutedFg);
    }
  }

  private static void RenderTreeView(RenderBuffer b)
  {
    Draw(b, 2, 1, "TreeView", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, "▾ src", AccentFg);
    Draw(b, 4, 4, "├─ TermBlade.Core", DefaultFg);
    Draw(b, 4, 5, "├─ TermBlade.Razor", DefaultFg);
    Draw(b, 4, 6, "└─ docs", DefaultFg);
    Draw(b, 7, 7, "└─ components.html", MutedFg);
  }

  private static void RenderTabSelect(RenderBuffer b)
  {
    Draw(b, 2, 1, "TabSelect", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, " Components ", Bg, TextAttributes.Bold, AccentFg);
    Draw(b, 14, 3, " Samples ", MutedFg, TextAttributes.None, PanelBg);
    Draw(b, 24, 3, " Docs ", MutedFg, TextAttributes.None, PanelBg);
    DrawBox(b, 2, 5, 34, 4, null, AccentFg, "single");
    Draw(b, 4, 6, "Active tab content", DefaultFg);
  }

  private static void RenderBox(RenderBuffer b)
  {
    DrawBox(b, 2, 1, 35, 6, " Box ", CyanFg, "rounded");
    Draw(b, 5, 3, "Rounded border + title", DefaultFg, TextAttributes.Bold);
    Draw(b, 5, 4, "Padding and styled content", MutedFg);
    DrawBox(b, 2, 8, 35, 3, null, MagentaFg, "double");
  }

  private static void RenderScrollBox(RenderBuffer b)
  {
    DrawBox(b, 2, 1, 38, 9, " ScrollBox ", BlueFg, "single");
    for (var i = 0; i < 6; i++) Draw(b, 4, 3 + i, $"Item {i + 8:00} in scrollable content", i == 2 ? AccentFg : DefaultFg);
    Draw(b, 38, 3, "█", AccentFg);
    Draw(b, 38, 4, "█", AccentFg);
    Draw(b, 38, 5, "░", MutedFg);
    Draw(b, 38, 6, "░", MutedFg);
    Draw(b, 38, 7, "░", MutedFg);
  }

  private static void RenderScrollBar(RenderBuffer b)
  {
    Draw(b, 2, 1, "ScrollBar", YellowFg, TextAttributes.Bold);
    Draw(b, 8, 3, "▲", MutedFg);
    Draw(b, 8, 4, "█", AccentFg);
    Draw(b, 8, 5, "█", AccentFg);
    Draw(b, 8, 6, "░", MutedFg);
    Draw(b, 8, 7, "░", MutedFg);
    Draw(b, 8, 8, "▼", MutedFg);
    Draw(b, 12, 5, "vertical track + thumb", DefaultFg);
  }

  private static void RenderConsoleOverlay(RenderBuffer b)
  {
    Draw(b, 2, 1, "ConsoleOverlay", YellowFg, TextAttributes.Bold);
    DrawBox(b, 2, 3, 42, 7, " Debug Console ", MagentaFg, "single");
    Draw(b, 4, 5, "> render frame 128", AccentFg);
    Draw(b, 4, 6, "> focused: Input#email", DefaultFg);
    Draw(b, 4, 7, "> fps: 60", MutedFg);
  }

  private static void RenderBarChart(RenderBuffer b)
  {
    Draw(b, 2, 1, "BarChart", YellowFg, TextAttributes.Bold);
    var heights = new[] { 3, 7, 5, 9, 4, 8, 6 };
    for (var i = 0; i < heights.Length; i++)
    {
      for (var y = 0; y < heights[i]; y++)
        Draw(b, 4 + i * 4, 10 - y, "██", i % 2 == 0 ? BlueFg : AccentFg);
    }

    Draw(b, 4, 11, "Mon Tue Wed Thu Fri Sat Sun", MutedFg);
  }

  private static void RenderLineChart(RenderBuffer b)
  {
    Draw(b, 2, 1, "LineChart", YellowFg, TextAttributes.Bold);
    Draw(b, 4, 9, "⣀", BlueFg);
    Draw(b, 6, 8, "⣠", BlueFg);
    Draw(b, 8, 7, "⣴", BlueFg);
    Draw(b, 10, 8, "⠙⢦", BlueFg);
    Draw(b, 14, 6, "⣠⠞", BlueFg);
    Draw(b, 18, 5, "⣴⠋", BlueFg);
    Draw(b, 22, 4, "⣠⠞", BlueFg);
    Draw(b, 4, 10, "area fill + Braille resolution", MutedFg);
  }

  private static void RenderTimeSeriesLineChart(RenderBuffer b)
  {
    Draw(b, 2, 1, "TimeSeriesLineChart", YellowFg, TextAttributes.Bold);
    Draw(b, 4, 8, "09:00", MutedFg);
    Draw(b, 18, 8, "11:00", MutedFg);
    Draw(b, 32, 8, "13:00", MutedFg);
    Draw(b, 4, 6, "⣀⣤⣤⣀", YellowFg);
    Draw(b, 12, 5, "⠉⠓⢦⣀", YellowFg);
    Draw(b, 20, 4, "⣀⣤⠞⠁", YellowFg);
    Draw(b, 28, 5, "⠉⢦⣀⣀", YellowFg);
    Draw(b, 4, 10, "timestamp labels on the X axis", MutedFg);
  }

  private static void RenderHeatMap(RenderBuffer b)
  {
    Draw(b, 2, 1, "HeatMap", YellowFg, TextAttributes.Bold);
    var colors = new[] { PanelBg, BlueFg, AccentFg, YellowFg, RedFg };
    for (var row = 0; row < 5; row++)
    {
      Draw(b, 2, 3 + row, $"R{row + 1}", MutedFg);
      for (var col = 0; col < 6; col++)
      {
        var bg = colors[(row + col) % colors.Length];
        Draw(b, 7 + col * 5, 3 + row, ((row + col + 1) * 3).ToString("00"), DefaultFg, TextAttributes.None, bg);
      }
    }
  }

  private static void RenderCandlestickChart(RenderBuffer b)
  {
    Draw(b, 2, 1, "CandlestickChart", YellowFg, TextAttributes.Bold);
    for (var i = 0; i < 8; i++)
    {
      var x = 5 + i * 4;
      var color = i % 2 == 0 ? AccentFg : RedFg;
      Draw(b, x, 3, "│", color);
      Draw(b, x, 4, "█", color);
      Draw(b, x, 5, "█", color);
      Draw(b, x, 6, "│", color);
    }

    Draw(b, 4, 9, "OHLC with bull and bear colors", MutedFg);
  }

  private static void RenderPieChart(RenderBuffer b)
  {
    Draw(b, 2, 1, "PieChart", YellowFg, TextAttributes.Bold);
    Draw(b, 6, 3, "  ██  ", BlueFg);
    Draw(b, 6, 4, "██████", BlueFg);
    Draw(b, 6, 5, "██", BlueFg);
    Draw(b, 8, 5, "████", AccentFg);
    Draw(b, 6, 6, "██████", YellowFg);
    Draw(b, 21, 4, "C# 35%", BlueFg);
    Draw(b, 21, 5, "TS 25%", AccentFg);
    Draw(b, 21, 6, "Py 20%", YellowFg);
  }

  private static void RenderDoughnutChart(RenderBuffer b)
  {
    Draw(b, 2, 1, "DoughnutChart", YellowFg, TextAttributes.Bold);
    Draw(b, 6, 3, " ████ ", BlueFg);
    Draw(b, 6, 4, "██  ██", BlueFg);
    Draw(b, 6, 5, "██OK██", AccentFg);
    Draw(b, 6, 6, "██  ██", YellowFg);
    Draw(b, 6, 7, " ████ ", YellowFg);
    Draw(b, 21, 5, "hollow center + label", MutedFg);
  }

  private static void RenderConfirm(RenderBuffer b)
  {
    DrawBox(b, 2, 1, 39, 8, " Confirm ", YellowFg, "rounded");
    Draw(b, 5, 3, "Delete selected files?", DefaultFg, TextAttributes.Bold);
    Draw(b, 5, 5, "  Yes  ", Bg, TextAttributes.Bold, RedFg);
    Draw(b, 14, 5, "  No  ", DefaultFg, TextAttributes.None, CursorBg);
    Draw(b, 5, 7, "Use arrows, Enter to confirm", MutedFg);
  }

  private static void RenderSpinner(RenderBuffer b)
  {
    Draw(b, 2, 1, "Spinner", YellowFg, TextAttributes.Bold);
    Draw(b, 2, 3, "⠋ Loading data...", CyanFg);
    Draw(b, 2, 5, "◐ Processing...", YellowFg);
    Draw(b, 2, 7, "▰▰▰▰▱▱▱▱▱▱ 40%", AccentFg);
    Draw(b, 2, 9, "Frames animate in terminal hosts", MutedFg);
  }

  private static void RenderMultiSelect(RenderBuffer b)
  {
    Draw(b, 2, 1, "MultiSelect", YellowFg, TextAttributes.Bold);
    var rows = new[] { "[x] Option A", "[ ] Option B", "[x] Option C", "[ ] Option D" };
    for (var i = 0; i < rows.Length; i++)
    {
      if (i == 1) Fill(b, 2, 3 + i, 24, CursorBg);
      Draw(b, 4, 3 + i, rows[i], rows[i].Contains("[x]") ? AccentFg : DefaultFg);
    }
  }

  private static void RenderUnknown(RenderBuffer b, string component)
  {
    Draw(b, 2, 2, "Unknown preview", RedFg, TextAttributes.Bold);
    Draw(b, 2, 4, component, DefaultFg);
  }

  private static void DrawBox(RenderBuffer b, int x, int y, int width, int height, string? title, Rgba color, string style)
  {
    b.DrawBorderWithTitles(x, y, width, height, style, color, Bg, title, "left", null, "left");
  }

  private static void Draw(RenderBuffer b, int x, int y, string text, Rgba fg, TextAttributes attrs = TextAttributes.None)
    => b.DrawText(x, y, text, fg, Bg, attrs);

  private static void Draw(RenderBuffer b, int x, int y, string text, Rgba fg, TextAttributes attrs, Rgba bg)
    => b.DrawText(x, y, text, fg, bg, attrs);

  private static void Fill(RenderBuffer b, int x, int y, int width, Rgba bg)
    => b.FillRect(x, y, width, 1, bg);

  private static string ToAnsi(RenderBuffer buffer)
  {
    var sb = new StringBuilder();
    Rgba? lastFg = null;
    Rgba? lastBg = null;
    TextAttributes? lastAttrs = null;

    sb.Append("\x1b[2J\x1b[H");
    for (var y = 0; y < buffer.Height; y++)
    {
      for (var x = 0; x < buffer.Width; x++)
      {
        var cell = buffer.GetCell(x, y) ?? default;
        if (lastAttrs is null || cell.Attributes != lastAttrs.Value)
        {
          sb.Append("\x1b[0m");
          AnsiCodes.WriteAttributes(new StringWriter(sb), cell.Attributes);
          lastFg = null;
          lastBg = null;
          lastAttrs = cell.Attributes;
        }

        if (lastFg is null || cell.Fg != lastFg.Value)
        {
          AnsiCodes.WriteFgColor(new StringWriter(sb), cell.Fg);
          lastFg = cell.Fg;
        }

        if (lastBg is null || cell.Bg != lastBg.Value)
        {
          AnsiCodes.WriteBgColor(new StringWriter(sb), cell.Bg);
          lastBg = cell.Bg;
        }

        if (cell.Codepoint == 0)
          sb.Append(' ');
        else
          sb.Append(char.ConvertFromUtf32(cell.Codepoint));
      }

      if (y < buffer.Height - 1)
        sb.Append("\r\n");
    }

    sb.Append("\x1b[0m");
    return sb.ToString();
  }
}
