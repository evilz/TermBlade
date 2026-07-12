using System.Text;
using TermBlade.Core.Ansi;

namespace TermBlade.Tests;

public sealed class AnsiCodesTests
{
  [Theory]
  [InlineData(12, 4)]
  [InlineData(1, 1)]
  public void AppendMoveTo_MatchesStringHelper(int column, int row)
  {
    var builder = new StringBuilder();

    AnsiCodes.AppendMoveTo(builder, column, row);

    Assert.Equal(AnsiCodes.MoveTo(column, row), builder.ToString());
  }

  [Theory]
  [InlineData(255, 12, 7)]
  [InlineData(0, 0, 0)]
  public void AppendColors_MatchWriterHelpers(byte red, byte green, byte blue)
  {
    var color = Rgba.FromInts(red, green, blue);
    var expected = new StringBuilder();
    using (var writer = new StringWriter(expected))
    {
      AnsiCodes.WriteFgColor(writer, color);
      AnsiCodes.WriteBgColor(writer, color);
    }

    var actual = new StringBuilder();
    AnsiCodes.AppendFgColor(actual, color);
    AnsiCodes.AppendBgColor(actual, color);

    Assert.Equal(expected.ToString(), actual.ToString());
  }

  [Fact]
  public void AppendAttributes_MatchesWriterHelper()
  {
    const TextAttributes attributes = TextAttributes.Bold | TextAttributes.Italic | TextAttributes.Underline;
    var expected = new StringBuilder();
    using (var writer = new StringWriter(expected))
      AnsiCodes.WriteAttributes(writer, attributes);

    var actual = new StringBuilder();
    AnsiCodes.AppendAttributes(actual, attributes);

    Assert.Equal(expected.ToString(), actual.ToString());
  }
}
