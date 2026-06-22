using System.Reflection;
using TermBlade.Core.Layout;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;
using TermBlade.Razor.Components;

namespace TermBlade.Tests;

public class ComponentInventoryTests
{
  public static TheoryData<string> CoreRenderableTypeNames => new()
  {
    "MarkupRenderable",
    "PanelRenderable",
    "TextPathRenderable",
    "TableRenderable",
    "TreeRenderable",
    "ColumnsRenderable",
    "RuleRenderable",
    "GridRenderable",
    "RowsRenderable",
    "LayoutRenderable",
    "PadderRenderable",
    "AlignRenderable",
    "FigletTextRenderable",
    "BreakdownChartRenderable",
    "JsonTextRenderable",
    "CanvasRenderable",
    "CanvasImageRenderable",
    "NotificationRenderable",
    "ButtonRenderable",
    "CalendarPickerRenderable",
    "CarouselRenderable",
    "CheckBoxRenderable",
    "DialogRenderable",
    "DataGridRenderable",
    "ExpanderRenderable",
    "FlyoutRenderable",
    "IconRenderable",
    "ImageRenderable",
    "MessageBoxRenderable",
    "NumericUpDownRenderable",
    "ProgressBarRenderable",
    "RadioButtonRenderable",
    "RelativePanelRenderable",
    "ScrollViewerRenderable",
    "TabRenderable",
    "ToggleSwitchRenderable",
    "ProgressDisplayRenderable",
    "StatusDisplayRenderable",
    "LiveDisplayRenderable",
    "TextPromptRenderable",
    "SelectionPromptRenderable",
    "MultiSelectionPromptRenderable",
    "AutoCompleteRenderable",
    "ComboBoxRenderable",
    "GradientBrushRenderable",
    "CalendarPikerRenderable",
    "DislogRenderable",
    "MassageBoxRenderable",
    "SrollViewerRenderable",
  };

  public static TheoryData<string> RazorComponentTypeNames => new()
  {
    "Markup",
    "Panel",
    "TextPath",
    "Table",
    "Tree",
    "Columns",
    "Rule",
    "Grid",
    "Rows",
    "Layout",
    "Padder",
    "Align",
    "FigletText",
    "BreakdownChart",
    "JsonText",
    "Canvas",
    "CanvasImage",
    "Notification",
    "Button",
    "CalendarPicker",
    "Carousel",
    "CheckBox",
    "Dialog",
    "DataGrid",
    "Expander",
    "Flyout",
    "Icon",
    "Image",
    "MessageBox",
    "NumericUpDown",
    "ProgressBar",
    "RadioButton",
    "RelativePanel",
    "ScrollViewer",
    "Tab",
    "ToggleSwitch",
    "ProgressDisplay",
    "StatusDisplay",
    "LiveDisplay",
    "TextPrompt",
    "SelectionPrompt",
    "MultiSelectionPrompt",
    "AutoComplete",
    "ComboBox",
    "GradientBrush",
    "CalendarPiker",
    "Dislog",
    "MassageBox",
    "SrollViewer",
  };

  [Theory]
  [MemberData(nameof(CoreRenderableTypeNames))]
  public void Core_renderable_exists(string typeName)
  {
    var type = typeof(TextRenderable).Assembly.GetType($"TermBlade.Core.Renderables.{typeName}", throwOnError: false);

    Assert.NotNull(type);
    Assert.True(typeof(Renderable).IsAssignableFrom(type), $"{typeName} should be a renderable.");
  }

  [Theory]
  [MemberData(nameof(CoreRenderableTypeNames))]
  public void Core_renderable_smoke_renders(string typeName)
  {
    var type = typeof(TextRenderable).Assembly.GetType($"TermBlade.Core.Renderables.{typeName}", throwOnError: true)!;
    var renderable = (Renderable)Activator.CreateInstance(type, [null])!;
    Layout(renderable, width: 60, height: 18);
    var buffer = new RenderBuffer(60, 18);

    var exception = Record.Exception(() => renderable.Render(buffer, 0.016));

    Assert.Null(exception);
  }

  [Theory]
  [MemberData(nameof(RazorComponentTypeNames))]
  public void Razor_component_exists(string typeName)
  {
    var type = typeof(Text).Assembly.GetType($"TermBlade.Razor.Components.{typeName}", throwOnError: false);

    Assert.NotNull(type);
  }

  [Theory]
  [MemberData(nameof(CoreRenderableTypeNames))]
  public void Core_renderable_has_xml_summary(string typeName)
  {
    var sourcePath = Path.Combine(GetRepositoryRoot(), "src", "TermBlade.Core", "Renderables", $"{typeName}.cs");
    Assert.True(File.Exists(sourcePath), $"{typeName} should be declared in its own file.");

    var source = File.ReadAllText(sourcePath);

    Assert.Contains($"public class {typeName}", source);
    var declarationIndex = source.IndexOf($"public class {typeName}", StringComparison.Ordinal);
    var summaryIndex = source.LastIndexOf("/// <summary>", declarationIndex, StringComparison.Ordinal);
    Assert.True(summaryIndex >= 0 && declarationIndex - summaryIndex < 400, $"{typeName} should have nearby XML documentation.");
  }

  [Theory]
  [MemberData(nameof(RazorComponentTypeNames))]
  public void Razor_component_has_xml_summary(string typeName)
  {
    var componentsDirectory = Path.Combine(GetRepositoryRoot(), "src", "TermBlade.Razor", "Components");
    var sourcePath = Path.Combine(componentsDirectory, $"{typeName}.cs");
    var razorPath = Path.Combine(componentsDirectory, $"{typeName}.razor");

    Assert.True(File.Exists(sourcePath) || File.Exists(razorPath), $"{typeName} should be declared in its own file.");

    if (File.Exists(razorPath))
    {
      var docsPath = Path.Combine(GetRepositoryRoot(), "docs", "components.html");
      var apiPath = Path.Combine(GetRepositoryRoot(), "docs", "api.html");
      var componentsHtml = File.ReadAllText(docsPath);
      var apiHtml = File.ReadAllText(apiPath);

      Assert.Contains($"<h2>{typeName}</h2>", componentsHtml);
      Assert.Contains($"TermBlade.Razor.Components.{typeName}", apiHtml);
      return;
    }

    var source = File.ReadAllText(sourcePath);

    Assert.Contains($"public sealed class {typeName}", source);
    var declarationIndex = source.IndexOf($"public sealed class {typeName}", StringComparison.Ordinal);
    var summaryIndex = source.LastIndexOf("/// <summary>", declarationIndex, StringComparison.Ordinal);
    Assert.True(summaryIndex >= 0 && declarationIndex - summaryIndex < 400, $"{typeName} should have nearby XML documentation.");
  }

  private static string GetRepositoryRoot()
  {
    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "TermBlade.slnx")))
      directory = directory.Parent;

    return directory?.FullName ?? throw new InvalidOperationException("Could not locate repository root.");
  }

  private static void Layout(Renderable renderable, int width, int height)
  {
    var root = new FlexNode();
    renderable.SetWidth(width);
    renderable.SetHeight(height);
    root.AddChild(renderable.LayoutNode);
    FlexLayout.Calculate(root, width, height);
  }
}
