using TermBlade.Core.Rendering;

namespace TermBlade.Tests;

public class CliRendererTests
{
  private const uint EnableProcessedInput = 0x0001;
  private const uint EnableLineInput = 0x0002;
  private const uint EnableEchoInput = 0x0004;
  private const uint EnableWindowInput = 0x0008;
  private const uint EnableMouseInput = 0x0010;
  private const uint EnableQuickEditMode = 0x0040;
  private const uint EnableExtendedFlags = 0x0080;

  private const uint EnableProcessedOutput = 0x0001;
  private const uint EnableVirtualTerminalProcessing = 0x0004;

  [Fact]
  public void BuildWindowsRawInputMode_DisablesEchoAndLineInput()
  {
    const uint initialMode =
        EnableProcessedInput |
        EnableLineInput |
        EnableEchoInput |
        EnableQuickEditMode;

    var mode = CliRenderer.BuildWindowsRawInputMode(initialMode);

    Assert.Equal(0u, mode & EnableEchoInput);
    Assert.Equal(0u, mode & EnableLineInput);
    Assert.Equal(0u, mode & EnableProcessedInput);
    Assert.Equal(0u, mode & EnableQuickEditMode);
    Assert.NotEqual(0u, mode & EnableExtendedFlags);
    Assert.NotEqual(0u, mode & EnableMouseInput);
    Assert.NotEqual(0u, mode & EnableWindowInput);
  }

  [Fact]
  public void BuildWindowsRawOutputMode_EnablesVirtualTerminalProcessing()
  {
    var mode = CliRenderer.BuildWindowsRawOutputMode(0);

    Assert.NotEqual(0u, mode & EnableProcessedOutput);
    Assert.NotEqual(0u, mode & EnableVirtualTerminalProcessing);
  }

  [Fact]
  public void ResolveConsoleSize_UsesLargerNativeWindow_WhenConsoleWindowIsStale()
  {
    var size = CliRenderer.ResolveConsoleSize(
        new CliRenderer.ConsoleSize(120, 40),
        new CliRenderer.ConsoleSize(190, 72),
        new CliRenderer.ConsoleSize(80, 24));

    Assert.Equal(190, size.Width);
    Assert.Equal(72, size.Height);
  }

  [Fact]
  public void ResolveConsoleSize_UsesLargerTerminalReport_WhenConsoleApisAreStale()
  {
    var size = CliRenderer.ResolveConsoleSize(
        new CliRenderer.ConsoleSize(120, 40),
        new CliRenderer.ConsoleSize(120, 40),
        new CliRenderer.ConsoleSize(190, 72),
        new CliRenderer.ConsoleSize(80, 24));

    Assert.Equal(190, size.Width);
    Assert.Equal(72, size.Height);
  }

  [Fact]
  public void ResolveConsoleSize_IgnoresInvalidSources()
  {
    var size = CliRenderer.ResolveConsoleSize(
        new CliRenderer.ConsoleSize(0, 40),
        new CliRenderer.ConsoleSize(190, 0),
        new CliRenderer.ConsoleSize(80, 24));

    Assert.Equal(80, size.Width);
    Assert.Equal(24, size.Height);
  }

  [Fact]
  public void TryParseTerminalSizeReport_ParsesWindowReport()
  {
    Assert.True(CliRenderer.TryParseTerminalSizeReport("\x1b[8;72;190t", out var size));
    Assert.Equal(190, size.Width);
    Assert.Equal(72, size.Height);
  }

  [Fact]
  public void TryParseTerminalSizeReport_IgnoresMalformedReport()
  {
    Assert.False(CliRenderer.TryParseTerminalSizeReport("\x1b[8;0;190t", out _));
  }

  [Fact]
  public void ParseWindowsKey_PrintableCharacter_ReturnsKeyEvent()
  {
    var key = CliRenderer.ParseWindowsKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));

    Assert.NotNull(key);
    Assert.Equal("a", key.Name);
    Assert.Equal('a', key.Char);
  }

  [Fact]
  public void ParseWindowsKey_CtrlC_ReturnsCtrlCombination()
  {
    var key = CliRenderer.ParseWindowsKey(new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, true));

    Assert.NotNull(key);
    Assert.Equal("ctrl+c", key.Name);
    Assert.True(key.Ctrl);
  }

  [Fact]
  public void ParseWindowsKey_Arrow_ReturnsNormalizedName()
  {
    var key = CliRenderer.ParseWindowsKey(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));

    Assert.NotNull(key);
    Assert.Equal("left", key.Name);
    Assert.Null(key.Char);
  }
}
