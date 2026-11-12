using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Tests;

public class TutorialDocumentationTests
{
  [Fact]
  public void LearnTutorialDashboard_ExplicitlySizesEveryVisibleRow()
  {
    var dashboardCode = ReadCodeBlock("dashboard-code");

    Assert.Contains("<Text Height=\"1\" Content=\"TermBlade dashboard\"", dashboardCode);
    Assert.Contains("<Text Height=\"1\" Content=\"Press ↑ or ↓ to change the selected item.\"", dashboardCode);
    Assert.Contains("<Box Height=\"4\" MarginTop=\"1\" Border=\"true\"", dashboardCode);
    Assert.Contains("<Text Height=\"1\" Content=\"@Items[SelectedIndex].Name\"", dashboardCode);
  }

  [Fact]
  public void LearnTutorialFinalDashboard_IsTheRunnableSampleSource()
  {
    var tutorialCode = ReadCodeBlock("input-code");
    var samplePath = Path.Combine(FindRepositoryRoot(), "samples", "TermBlade.Razor.Samples", "Components", "TutorialDashboardSample.razor");
    var sampleCode = NormalizeNewlines(File.ReadAllText(samplePath)).Trim();

    Assert.Equal(sampleCode, tutorialCode);
    Assert.Contains("AutoFocus=\"true\"", tutorialCode);
    Assert.Contains("SelectedIndexChanged=\"@OnSelectionChanged\"", tutorialCode);
    Assert.Contains("private Task OnSelectionChanged(int index)", tutorialCode);
  }

  [Fact]
  public void LearnTutorialDashboard_RendersOnCompactTerminal()
  {
    using var renderer = CreateTestingRenderer(width: 40, height: 8);
    renderer.Root.Add(BuildInitialDashboard(renderer));

    var frame = RenderFrame(renderer);

    Assert.Contains("TermBlade dashboard", frame);
    Assert.Contains("Press ↑ or ↓ to change the selected item.", frame);
    Assert.Contains("Build", frame);
  }

  [Fact]
  public void LearnTutorialSelection_ChangesOnArrowKeyAndUpdatesFooter()
  {
    using var renderer = CreateTestingRenderer(width: 40, height: 8);
    var dashboard = BuildInteractiveDashboard(renderer, out var selection, out var status);
    renderer.Root.Add(dashboard);
    selection.Focus();

    renderer.DispatchKey(new KeyEvent { Name = "down", Key = "down" });

    var frame = RenderFrame(renderer);

    Assert.Equal(1, selection.SelectedIndex);
    Assert.Equal("Selected: Test", status.Content);
    Assert.Contains("▶ Test", frame);
    Assert.Contains("Selected: Test", frame);
  }

  private static BoxRenderable BuildInitialDashboard(CliRenderer renderer)
  {
    var dashboard = CreateDashboard(renderer);
    dashboard.Add(CreateText(renderer, "TermBlade dashboard", height: 1, fg: "#58a6ff", bold: true));
    dashboard.Add(CreateText(renderer, "Press ↑ or ↓ to change the selected item.", height: 1, fg: "#8b949e"));

    var panel = new BoxRenderable(renderer, new BoxOptions
    {
      Height = "4",
      Border = true,
      BorderColor = "#30363d",
      PaddingLeft = 1,
      PaddingTop = 1,
      FlexDirection = FlexDirection.Column
    })
    {
      MarginTop = 1
    };
    panel.Add(CreateText(renderer, "Build", height: 1, fg: "#ffffff"));
    dashboard.Add(panel);
    return dashboard;
  }

  private static BoxRenderable BuildInteractiveDashboard(
    CliRenderer renderer,
    out SelectRenderable selection,
    out TextRenderable status)
  {
    var dashboard = CreateDashboard(renderer);
    dashboard.Add(CreateText(renderer, "TermBlade dashboard", height: 1, fg: "#58a6ff", bold: true));
    dashboard.Add(CreateText(renderer, "Use ↑ or ↓ to change the selected item.", height: 1, fg: "#8b949e"));

    var items = new List<SelectOption>
    {
      new() { Name = "Build", Description = "Compile the application", Value = "build" },
      new() { Name = "Test", Description = "Run the test suite", Value = "test" },
      new() { Name = "Ship", Description = "Create a release", Value = "ship" }
    };
    selection = new SelectRenderable(renderer)
    {
      Options = items,
      SelectedIndex = 0
    };
    selection.SetWidth("30");
    selection.SetHeight("4");
    var statusText = CreateText(renderer, "Selected: Build", height: 1, fg: "#8b949e");
    status = statusText;
    selection.On("selectionChanged", value =>
    {
      int selectedIndex = Convert.ToInt32(value ?? 0);
      statusText.Content = $"Selected: {items[selectedIndex].Name}";
    });

    dashboard.Add(selection);
    dashboard.Add(status);
    return dashboard;
  }

  private static BoxRenderable CreateDashboard(CliRenderer renderer)
    => new(renderer, new BoxOptions
    {
      Width = "100%",
      Height = "100%",
      BackgroundColor = "#0d1117",
      FlexDirection = FlexDirection.Column,
      PaddingLeft = 2,
      PaddingTop = 1
    });

  private static TextRenderable CreateText(
    CliRenderer renderer,
    string content,
    int height,
    string fg,
    bool bold = false)
    => new(renderer, new TextOptions
    {
      Content = content,
      Height = height,
      Fg = fg,
      Attributes = bold ? TermBlade.Core.Ansi.TextAttributes.Bold : TermBlade.Core.Ansi.TextAttributes.None
    });

  private static CliRenderer CreateTestingRenderer(int width, int height)
    => new(new CliRendererConfig
    {
      Testing = true,
      TestingWidth = width,
      TestingHeight = height
    });

  private static string RenderFrame(CliRenderer renderer)
  {
    FlexLayout.Calculate(renderer.Root.LayoutNode, renderer.TerminalWidth, renderer.TerminalHeight);
    var buffer = new RenderBuffer(renderer.TerminalWidth, renderer.TerminalHeight);
    renderer.Root.Render(buffer, 0);

    var frame = new StringBuilder();
    for (int y = 0; y < buffer.Height; y++)
    {
      for (int x = 0; x < buffer.Width; x++)
      {
        int codepoint = buffer.GetCell(x, y)!.Value.Codepoint;
        frame.Append(codepoint == 0 ? " " : char.ConvertFromUtf32(codepoint));
      }

      frame.AppendLine();
    }

    return frame.ToString();
  }

  private static string ReadCodeBlock(string id)
  {
    var html = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "docs", "learn.html"));
    string pattern = "<pre id=\"" + Regex.Escape(id) + "\" class=\"code-block\"><code>(.*?)</code></pre>";
    var match = Regex.Match(html, pattern, RegexOptions.Singleline);

    Assert.True(match.Success, $"Could not find the {id} code block in learn.html.");
    return NormalizeNewlines(WebUtility.HtmlDecode(match.Groups[1].Value)).Trim();
  }

  private static string NormalizeNewlines(string value)
    => value.Replace("\r\n", "\n", StringComparison.Ordinal);

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
}
