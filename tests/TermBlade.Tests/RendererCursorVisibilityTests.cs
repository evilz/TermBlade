using System.IO;
using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;
using Xunit;

namespace TermBlade.Tests;

public class RendererCursorVisibilityTests
{
  [Fact]
  public void Render_EmitsExactCursorVisibilityTransitions()
  {
    using var output = new StringWriter();
    using var renderer = new Renderer(5, 3, output);
    renderer.Initialize();
    output.GetStringBuilder().Clear();

    renderer.SetCursorPosition(2, 1, visible: true);
    renderer.Render();
    Assert.Equal("\u001b[?25h\u001b[2;3H", output.ToString());

    output.GetStringBuilder().Clear();
    renderer.SetCursorPosition(2, 1, visible: false);
    renderer.Render();
    Assert.Equal("\u001b[?25l", output.ToString());

    output.GetStringBuilder().Clear();
    renderer.Render();
    Assert.Empty(output.ToString());
  }

  [Fact]
  public void Render_WithContentAndVisibleCursorTransition_EmitsFullFrameThenOnlyCursorOnUnchangedFrame()
  {
    using var output = new StringWriter();
    using var renderer = new Renderer(5, 3, output);
    renderer.Initialize();
    output.GetStringBuilder().Clear();

    var foreground = Rgba.FromInts(255, 255, 255);
    var background = Rgba.FromInts(0, 0, 0);
    var frame = renderer.GetFrameBuffer();
    frame.SetCell(0, 0, 'A', foreground, background);
    frame.SetCell(1, 0, 'B', foreground, background);
    renderer.SetCursorPosition(2, 1, visible: true);

    renderer.Render();

    Assert.Equal(
        "\u001b[1;1H\u001b[0m\u001b[38;2;255;255;255m\u001b[48;2;0;0;0mA\u001b[1;2HB\u001b[?25h\u001b[2;3H",
        output.ToString());

    output.GetStringBuilder().Clear();
    renderer.Render();

    Assert.Equal("\u001b[2;3H", output.ToString());
  }

  [Fact]
  public void Render_ClampsVisibleCursorAfterResize()
  {
    using var output = new StringWriter();
    using var renderer = new Renderer(5, 3, output);
    renderer.Initialize();
    renderer.SetCursorPosition(4, 2, visible: true);
    renderer.Resize(2, 1);
    output.GetStringBuilder().Clear();

    renderer.Render();

    Assert.Equal("\u001b[?25h\u001b[1;2H", output.ToString());
  }

  [Fact]
  public void Shutdown_RestoresCursorAndIsIdempotent()
  {
    using var output = new StringWriter();
    using var renderer = new Renderer(5, 3, output);
    renderer.Initialize();
    renderer.SetCursorPosition(0, 0, visible: false);
    renderer.Render();
    output.GetStringBuilder().Clear();

    renderer.Shutdown();
    Assert.Equal("\u001b[?25h\u001b[?1049l\u001b[0m", output.ToString());

    output.GetStringBuilder().Clear();
    renderer.Shutdown();
    Assert.Empty(output.ToString());
  }
}
