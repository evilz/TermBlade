using System.Text;
using System.Text.RegularExpressions;
using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
using TermBlade.Core.Rendering;

namespace TermBlade.Tests;

public class PlainTextControlSafetyTests
{
  private const string UntrustedText = "before\0\a\u001B]52;c;termblade\a\u009B31m\t\r\nafter";

  [Fact]
  public void CellBuffer_DrawTextReplacesTerminalControlsBeforeCapturingFrame()
  {
    using var buffer = CellBuffer.Create(80, 1);
    buffer.DrawText(UntrustedText, 0, 0, Rgba.FromInts(255, 255, 255));

    var captured = Encoding.UTF8.GetString(buffer.GetRealCharBytes());

    Assert.Equal(ToPrintableText(UntrustedText), captured[..ToPrintableText(UntrustedText).Length]);
    Assert.DoesNotContain(captured, character => char.IsControl(character));
  }

  [Fact]
  public void CellBuffer_SetCellReplacesControlsAndKeepsContinuationMarker()
  {
    using var buffer = CellBuffer.Create(2, 1);
    var white = Rgba.FromInts(255, 255, 255);
    var black = Rgba.FromInts(0, 0, 0);

    buffer.SetCell(0, 0, 0x1B, white, black);
    buffer.SetCell(1, 0, 0, white, black);

    Assert.Equal(0xFFFD, buffer.GetCell(0, 0)!.Value.Codepoint);
    Assert.Equal(0, buffer.GetCell(1, 0)!.Value.Codepoint);
  }

  [Fact]
  public void RenderBuffer_DrawTextReplacesControlsBeforeCliDiffEmission()
  {
    var buffer = new RenderBuffer(80, 1);
    buffer.DrawText(0, 0, UntrustedText, Rgba.FromInts(255, 255, 255), Rgba.FromInts(0, 0, 0));

    var captured = new StringBuilder();
    for (int x = 0; x < buffer.Width; x++)
    {
      var codepoint = buffer.GetCell(x, 0)!.Value.Codepoint;
      if (codepoint != 0)
        captured.Append(char.ConvertFromUtf32(codepoint));
    }

    Assert.Equal(ToPrintableText(UntrustedText), captured.ToString().TrimEnd());
    Assert.DoesNotContain(captured.ToString(), character => char.IsControl(character));
  }

  [Fact]
  public void Renderer_DoesNotEmitUntrustedControlsFromPlainTextCells()
  {
    using var output = new StringWriter();
    using var renderer = new Renderer(80, 1, output) { UseAlternateScreen = false };
    renderer.Initialize();
    output.GetStringBuilder().Clear();

    renderer.GetFrameBuffer().DrawText(
        UntrustedText,
        0,
        0,
        Rgba.FromInts(255, 255, 255),
        Rgba.FromInts(0, 0, 0));
    renderer.Render();

    var plainOutput = Regex.Replace(output.ToString(), @"\x1B\[[0-?]*[ -/]*[@-~]", string.Empty);

    Assert.Equal(ToPrintableText(UntrustedText), plainOutput);
    Assert.DoesNotContain(plainOutput, character => char.IsControl(character));
  }

  private static string ToPrintableText(string value)
    => new(value.Select(character => char.IsControl(character) ? '\uFFFD' : character).ToArray());
}
