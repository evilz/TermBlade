using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
using TermBlade.Core.Rendering;

namespace TermBlade.Tests;

public class BufferDimensionTests
{
  [Theory]
  [InlineData(0, 1)]
  [InlineData(-1, 1)]
  [InlineData(1, 0)]
  [InlineData(1, -1)]
  [InlineData(65_536, 65_536)]
  [InlineData(int.MaxValue, 1)]
  public void CellBuffer_RejectsInvalidOrUnsupportedDimensions(int width, int height)
  {
    Assert.Throws<ArgumentOutOfRangeException>(() => new CellBuffer(width, height));
  }

  [Theory]
  [InlineData(0, 1)]
  [InlineData(-1, 1)]
  [InlineData(1, 0)]
  [InlineData(1, -1)]
  [InlineData(65_536, 65_536)]
  [InlineData(int.MaxValue, 1)]
  public void RenderBuffer_RejectsInvalidOrUnsupportedDimensions(int width, int height)
  {
    Assert.Throws<ArgumentOutOfRangeException>(() => new RenderBuffer(width, height));
  }

  [Fact]
  public void CellBuffer_InvalidResizeLeavesExistingDimensionsAndCellsIntact()
  {
    using var buffer = new CellBuffer(2, 3);
    var foreground = Rgba.FromInts(255, 255, 255);
    var background = Rgba.FromInts(0, 0, 0);
    buffer.SetCell(1, 2, 'X', foreground, background);

    Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Resize(65_536, 65_536));

    Assert.Equal(2, buffer.Width);
    Assert.Equal(3, buffer.Height);
    Assert.Equal('X', buffer.GetCell(1, 2)!.Value.Codepoint);
  }

  [Fact]
  public void BufferConstructorsAcceptSmallPositiveDimensions()
  {
    using var cellBuffer = new CellBuffer(1, 1);
    var renderBuffer = new RenderBuffer(1, 1);

    Assert.Equal(1, cellBuffer.Width);
    Assert.Equal(1, cellBuffer.Height);
    Assert.Equal(1, renderBuffer.Width);
    Assert.Equal(1, renderBuffer.Height);
  }
}
