using System.Reflection;

namespace TermBlade.Gallery.Components;

/// <summary>
/// Registry of all gallery component entries with their metadata and embedded source code.
/// </summary>
public static class GalleryCatalog
{
  private static readonly List<GalleryEntry> _entries =
  [
      new("Box",
            "Container with borders, background color, padding and flex layout.",
            "Components/Demos/BoxDemo.razor",
            ReadDemo("BoxDemo.razor")),

        new("Text",
            "Renders styled text with colors, bold, italic, underline and more.",
            "Components/Demos/TextDemo.razor",
            ReadDemo("TextDemo.razor")),

        new("Code",
            "Displays syntax-highlighted source code with line numbers.",
            "Components/Demos/CodeDemo.razor",
            ReadDemo("CodeDemo.razor")),

        new("AsciiFont",
            "Renders large ASCII art text using block fonts.",
            "Components/Demos/AsciiFontDemo.razor",
            ReadDemo("AsciiFontDemo.razor")),

        new("Select",
            "A navigable selection list with descriptions.",
            "Components/Demos/SelectDemo.razor",
            ReadDemo("SelectDemo.razor")),

        new("Input",
            "Single-line text input field with placeholder and cursor.",
            "Components/Demos/InputDemo.razor",
            ReadDemo("InputDemo.razor")),

        new("Slider",
            "A horizontal slider control for numeric values.",
            "Components/Demos/SliderDemo.razor",
            ReadDemo("SliderDemo.razor")),

        new("TabSelect",
            "Horizontal tab selector for switching views.",
            "Components/Demos/TabSelectDemo.razor",
            ReadDemo("TabSelectDemo.razor")),

        new("Markdown",
            "Renders markdown content with headings, lists and code blocks.",
            "Components/Demos/MarkdownDemo.razor",
            ReadDemo("MarkdownDemo.razor")),

        new("Diff",
            "Side-by-side diff viewer with line numbers.",
            "Components/Demos/DiffDemo.razor",
            ReadDemo("DiffDemo.razor")),

        new("ScrollBox",
            "Scrollable container with vertical and horizontal scrollbars.",
            "Components/Demos/ScrollBoxDemo.razor",
            ReadDemo("ScrollBoxDemo.razor")),

        new("BarChart",
            "Vertical and horizontal bar charts with sparkline mode.",
            "Components/Demos/BarChartDemo.razor",
            ReadDemo("BarChartDemo.razor")),

        new("LineChart",
            "Line chart with Braille sub-cell resolution and area fill.",
            "Components/Demos/LineChartDemo.razor",
            ReadDemo("LineChartDemo.razor")),

        new("TimeSeriesLineChart",
            "Time-series line chart with timestamp X-axis labels.",
            "Components/Demos/TimeSeriesLineChartDemo.razor",
            ReadDemo("TimeSeriesLineChartDemo.razor")),

        new("HeatMap",
            "2D heat map grid with configurable color gradients.",
            "Components/Demos/HeatMapDemo.razor",
            ReadDemo("HeatMapDemo.razor")),

        new("CandlestickChart",
            "Financial OHLC candlestick chart with bull/bear coloring.",
            "Components/Demos/CandlestickChartDemo.razor",
            ReadDemo("CandlestickChartDemo.razor")),

        new("PieChart",
            "Pie chart with proportional slices and sweep animation.",
            "Components/Demos/PieChartDemo.razor",
            ReadDemo("PieChartDemo.razor")),

        new("DoughnutChart",
            "Doughnut chart with hollow center and optional center label.",
            "Components/Demos/DoughnutChartDemo.razor",
            ReadDemo("DoughnutChartDemo.razor")),

        new("TreeView",
            "Hierarchical tree with expand/collapse, checkboxes, filtering and mouse interaction.",
            "Components/Demos/TreeViewDemo.razor",
            ReadDemo("TreeViewDemo.razor")),

        new("Calendar",
            "Monthly calendar widget with highlighted date and alternating row backgrounds.",
            "Components/Demos/CalendarDemo.razor",
            ReadDemo("CalendarDemo.razor")),

        new("Textarea",
            "Multi-line text editor with word wrapping and cursor navigation.",
            "Components/Demos/TextareaDemo.razor",
            ReadDemo("TextareaDemo.razor")),

        new("Confirm",
            "A yes/no confirmation prompt with keyboard navigation.",
            "Components/Demos/ConfirmDemo.razor",
            ReadDemo("ConfirmDemo.razor")),

        new("Spinner",
            "An animated spinner indicator with customizable frames and title.",
            "Components/Demos/SpinnerDemo.razor",
            ReadDemo("SpinnerDemo.razor")),

        new("MultiSelect",
            "A selection list allowing multiple items to be toggled on or off.",
            "Components/Demos/MultiSelectDemo.razor",
            ReadDemo("MultiSelectDemo.razor")),
    ];

  public static IReadOnlyList<GalleryEntry> Entries => _entries;

  private static string ReadDemo(string fileName)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var resourceName = assembly.GetManifestResourceNames()
        .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

    if (resourceName is null)
      return $"// Source not found: {fileName}";

    using var stream = assembly.GetManifestResourceStream(resourceName)!;
    using var reader = new StreamReader(stream);
    return reader.ReadToEnd();
  }
}
