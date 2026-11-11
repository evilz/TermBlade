using System.IO;
using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
using TermBlade.Core.Rendering;
using Xunit;

namespace TermBlade.Tests;

public class RendererTests
{
  [Fact]
  public void Render_GraphemeClusterHasExactAnsiFrameAndIsUnchangedOnSecondRender()
  {
    using var output = new StringWriter();
    using var renderer = new Renderer(3, 1, output);
    var white = Rgba.FromInts(255, 255, 255);
    var black = Rgba.FromInts(0, 0, 0);
    renderer.GetFrameBuffer().DrawText("e\u0301X", 0, 0, white, black);

    renderer.Render();
    var firstFrame = output.ToString();

    Assert.Equal(
        "\u001b[1;1H\u001b[0m\u001b[38;2;255;255;255m\u001b[48;2;0;0;0me\u0301\u001b[1;2HX\u001b[?25l",
        firstFrame);

    renderer.Render();

    Assert.Equal(AnsiCodes.HideCursor, output.ToString()[firstFrame.Length..]);
  }
}