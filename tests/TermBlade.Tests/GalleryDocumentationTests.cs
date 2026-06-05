using System.Text.RegularExpressions;
using TermBlade.Docs.Wasm.Terminal.Previews;
using TermBlade.Gallery.Components;

namespace TermBlade.Tests;

public partial class GalleryDocumentationTests
{
  [Fact]
  public void ComponentsDocumentation_HasSameSamplesAsGalleryCatalog()
  {
    var documented = ReadDocumentedComponents();
    var gallery = GalleryCatalog.Entries
        .Select(entry => entry.Name)
        .Order(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    Assert.Equal(gallery, documented);
  }

  [Fact]
  public void ComponentPreviewService_RendersEveryGallerySample()
  {
    foreach (var entry in GalleryCatalog.Entries)
    {
      var ansi = ComponentPreviewService.RenderPreview(entry.Name);

      Assert.DoesNotContain("Unknown preview", ansi);
      Assert.Contains(entry.Name, ansi);
    }
  }

  [Fact]
  public void ComponentsDocumentation_ReferencesEveryGalleryDemoSource()
  {
    var root = FindRepositoryRoot();
    var componentsHtml = File.ReadAllText(Path.Combine(root, "docs", "components.html"));

    foreach (var entry in GalleryCatalog.Entries)
    {
      var fileName = Path.GetFileName(entry.SourceFile);

      Assert.Contains($"Demo Source <small>({fileName})</small>", componentsHtml);
    }
  }

  [Theory]
  [InlineData(0, 5, 0, 5)]
  [InlineData(4, 5, 0, 5)]
  [InlineData(5, 5, 1, 5)]
  [InlineData(23, 5, 19, 5)]
  public void GalleryMenuWindow_KeepsSelectedEntryVisible(int selectedIndex, int capacity, int expectedStart, int expectedCount)
  {
    var window = GalleryMenuWindow.Create(GalleryCatalog.Entries.Count, selectedIndex, capacity);

    Assert.Equal(expectedStart, window.StartIndex);
    Assert.Equal(expectedCount, window.Count);
    Assert.InRange(selectedIndex, window.StartIndex, window.StartIndex + window.Count - 1);
  }

  [Fact]
  public void GalleryMenuWindow_ShowsAllEntries_WhenCapacityExceedsCatalogSize()
  {
    var window = GalleryMenuWindow.Create(GalleryCatalog.Entries.Count, selectedIndex: 23, capacity: 99);

    Assert.Equal(0, window.StartIndex);
    Assert.Equal(GalleryCatalog.Entries.Count, window.Count);
  }

  private static string[] ReadDocumentedComponents()
  {
    var root = FindRepositoryRoot();
    var componentsHtml = File.ReadAllText(Path.Combine(root, "docs", "components.html"));

    return ComponentSectionRegex()
        .Matches(componentsHtml)
        .Select(match => match.Groups["name"].Value)
        .Order(StringComparer.OrdinalIgnoreCase)
        .ToArray();
  }

  private static string FindRepositoryRoot()
  {
    var directory = AppContext.BaseDirectory;

    while (directory is not null)
    {
      if (File.Exists(Path.Combine(directory, "TermBlade.slnx")))
        return directory;

      directory = Directory.GetParent(directory)?.FullName;
    }

    throw new InvalidOperationException("Could not find repository root.");
  }

  [GeneratedRegex("""<section id="(?<id>[^"]+)" class="component-section feature-card">\s*<header class="component-header">.*?<h2>(?<name>[^<]+)</h2>""", RegexOptions.Singleline)]
  private static partial Regex ComponentSectionRegex();
}
