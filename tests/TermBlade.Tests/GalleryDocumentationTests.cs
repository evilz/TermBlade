using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
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
  public void GalleryCatalog_IncludesEveryPublicRazorComponent()
  {
    var root = FindRepositoryRoot();
    var componentsPath = Path.Combine(root, "src", "TermBlade.Razor", "Components");
    var componentFiles = Directory.EnumerateFiles(componentsPath, "*.cs")
        .Concat(Directory.EnumerateFiles(componentsPath, "*.razor"))
        .Select(Path.GetFileNameWithoutExtension)
        .Where(name => name is not null
            and not "RenderableComponentBase"
            and not "ContainerRenderableComponentBase"
            and not "IRenderableParent"
            and not "_Imports")
        .Order(StringComparer.OrdinalIgnoreCase)
        .ToArray();
    var gallery = GalleryCatalog.Entries
        .Select(entry => entry.Name)
        .Order(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    Assert.Equal(componentFiles, gallery);
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

  [Fact]
  public void ApiDocumentation_ReferencesEveryPublicPackageType()
  {
    var root = FindRepositoryRoot();
    var apiHtml = File.ReadAllText(Path.Combine(root, "docs", "api.html"));
    var publicTypes = PublicPackageTypes().ToArray();

    foreach (var type in publicTypes)
    {
      Assert.Contains($"id=\"{HtmlId("T:" + XmlTypeName(type))}\"", apiHtml);
      Assert.Contains($">{System.Net.WebUtility.HtmlEncode(FriendlyTypeName(type))}</h3>", apiHtml);
    }

    Assert.Contains($"<strong>{publicTypes.Length}</strong> public types", apiHtml);
    Assert.DoesNotContain(">_Imports</h3>", apiHtml);
  }

  [Fact]
  public void DocumentationSite_LinksApiReferenceFromMainPages()
  {
    var root = FindRepositoryRoot();
    var indexHtml = File.ReadAllText(Path.Combine(root, "docs", "index.html"));
    var componentsHtml = File.ReadAllText(Path.Combine(root, "docs", "components.html"));
    var apiHtml = File.ReadAllText(Path.Combine(root, "docs", "api.html"));

    Assert.Contains("href=\"api.html\"", indexHtml);
    Assert.Contains("href=\"api.html\"", componentsHtml);
    Assert.Contains("href=\"components.html#text\"", apiHtml);
  }

  [Fact]
  public void ComponentPreviewScript_KeepsStaticFallbackWhenWasmPreviewIsUnavailable()
  {
    var root = FindRepositoryRoot();
    var componentsHtml = File.ReadAllText(Path.Combine(root, "docs", "components.html"));
    var previewScript = File.ReadAllText(Path.Combine(root, "docs", "js", "component-previews.js"));

    Assert.Contains("js/component-previews.js?v=", componentsHtml);
    Assert.Contains("wasm/index.html", previewScript);
    Assert.Contains("if (!wasmAvailable) return;", previewScript);
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

  private static IEnumerable<Type> PublicPackageTypes()
  {
    var assemblies = new[]
    {
      typeof(TermBlade.Core.Ansi.Rgba).Assembly,
      typeof(TermBlade.Razor.Components.Text).Assembly,
      typeof(TermBlade.FileManager.FileManagerApp).Assembly,
    };

    return assemblies
      .SelectMany(assembly => assembly.GetExportedTypes())
      .Where(type => type.Name is not "_Imports")
      .Where(type => type.FullName is not null && !type.FullName.Contains('<'))
      .Where(type => type.GetCustomAttribute<CompilerGeneratedAttribute>() is null
        || type.Namespace?.StartsWith("System.", StringComparison.Ordinal) is not true)
      .OrderBy(type => type.FullName, StringComparer.Ordinal);
  }

  private static string FriendlyTypeName(Type type)
  {
    if (type.IsGenericType)
    {
      var name = type.Name;
      var tick = name.IndexOf('`');
      if (tick >= 0)
        name = name[..tick];

      return name + "<" + string.Join(", ", type.GetGenericArguments().Select(FriendlyTypeName)) + ">";
    }

    return type.Name.Replace('+', '.');
  }

  private static string XmlTypeName(Type type)
  {
    if (type.IsGenericType)
    {
      var definition = type.GetGenericTypeDefinition();
      return (definition.FullName ?? definition.Name).Replace('+', '.');
    }

    return (type.FullName ?? type.Name).Replace('+', '.');
  }

  private static string HtmlId(string value)
  {
    var builder = new StringBuilder();

    foreach (var character in value.ToLowerInvariant())
    {
      if (char.IsLetterOrDigit(character))
        builder.Append(character);
      else if (character is '.' or ':' or '-' or '_' or '`')
        builder.Append('-');
    }

    var id = builder.ToString().Trim('-');
    while (id.Contains("--", StringComparison.Ordinal))
      id = id.Replace("--", "-");

    if (id.Length <= 90)
      return id;

    var hash = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(value)))[..8].ToLowerInvariant();
    return id[..80].Trim('-') + "-" + hash;
  }

  [GeneratedRegex("""<section id="(?<id>[^"]+)" class="component-section feature-card">\s*<header class="component-header">.*?<h2>(?<name>[^<]+)</h2>""", RegexOptions.Singleline)]
  private static partial Regex ComponentSectionRegex();
}
