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
