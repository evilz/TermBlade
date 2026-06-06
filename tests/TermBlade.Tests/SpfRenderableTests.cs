using TermBlade.Core.Ansi;
using TermBlade.Core.Layout;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Tests;

public class SpfRenderableTests
{
  [Fact]
  public void SegmentedText_RendersEachSegmentWithItsOwnStyle()
  {
    var renderable = new SegmentedTextRenderable(null)
    {
      BackgroundColor = "#1f1d2e",
      Segments =
      [
          new SegmentedTextSegment("\udb80\udc4b", "#7dd3fc"),
          new SegmentedTextSegment(" superfile", "#c4b5fd", Attributes: TextAttributes.Bold)
      ]
    };
    Layout(renderable, width: 24, height: 1);

    var buffer = new RenderBuffer(24, 1);
    renderable.Render(buffer, 0);

    Assert.Equal(Rgba.FromCss("#7dd3fc"), buffer.GetCell(0, 0)!.Value.Fg);
    Assert.Equal(Rgba.FromCss("#c4b5fd"), buffer.GetCell(2, 0)!.Value.Fg);
    Assert.Equal(TextAttributes.Bold, buffer.GetCell(2, 0)!.Value.Attributes);
  }

  [Fact]
  public void SegmentedText_AccountsForWideGlyphsWhenAdvancingColumns()
  {
    var renderable = new SegmentedTextRenderable(null)
    {
      Segments =
      [
          new SegmentedTextSegment("A", "#ffffff"),
          new SegmentedTextSegment("📁", "#a6e3a1"),
          new SegmentedTextSegment("B", "#ffffff")
      ]
    };
    Layout(renderable, width: 8, height: 1);

    var buffer = new RenderBuffer(8, 1);
    renderable.Render(buffer, 0);

    Assert.Equal('A', buffer.GetCell(0, 0)!.Value.Codepoint);
    Assert.Equal(0, buffer.GetCell(2, 0)!.Value.Codepoint);
    Assert.Equal('B', buffer.GetCell(3, 0)!.Value.Codepoint);
  }

  [Fact]
  public void DrawBorderWithTitles_SpfStyleUsesRoundedCornersAndBottomTitle()
  {
    var buffer = new RenderBuffer(16, 4);
    var fg = Rgba.FromCss("#b4befe");
    var bg = Rgba.FromCss("#1e1e2e");

    buffer.DrawBorderWithTitles(0, 0, 16, 4, "spf", fg, bg, " Name ", "left", " 1/6 ", "right");

    Assert.Equal('╭', buffer.GetCell(0, 0)!.Value.Codepoint);
    Assert.Equal('╮', buffer.GetCell(15, 0)!.Value.Codepoint);
    Assert.Contains("1/6", ReadRow(buffer, 3));
  }

  private static void Layout(Renderable renderable, int width, int height)
  {
    var root = new FlexNode();
    renderable.SetWidth(width);
    renderable.SetHeight(height);
    root.AddChild(renderable.LayoutNode);
    FlexLayout.Calculate(root, width, height);
  }

  private static string ReadRow(RenderBuffer buffer, int y)
  {
    var chars = new List<string>();
    for (var x = 0; x < buffer.Width; x++)
    {
      var codepoint = buffer.GetCell(x, y)!.Value.Codepoint;
      if (codepoint != 0)
        chars.Add(char.ConvertFromUtf32(codepoint));
    }

    return string.Concat(chars);
  }
}
