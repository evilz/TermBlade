using System.Text;
using TermBlade.Core.Input;

namespace TermBlade.Tests;

public class StdinParserTests
{
  [Theory]
  [InlineData("\u001b[1;5D", "left", true, false, false)]
  [InlineData("\u001b[1;3C", "alt+right", false, true, false)]
  [InlineData("\u001b[1;2A", "up", false, false, true)]
  [InlineData("\u001b[1;8H", "alt+home", true, true, true)]
  [InlineData("\u001b[1;6F", "end", true, false, true)]
  public void Feed_ParsesModifiersOnArrowsAndHomeEnd(
      string sequence,
      string expectedName,
      bool expectedCtrl,
      bool expectedAlt,
      bool expectedShift)
  {
    var parser = new StdinParser();

    var key = Assert.IsType<KeyEvent>(Assert.Single(parser.Feed(Encoding.ASCII.GetBytes(sequence))));

    Assert.Equal(expectedName, key.Name);
    Assert.Equal(expectedCtrl, key.Ctrl);
    Assert.Equal(expectedAlt, key.Alt);
    Assert.Equal(expectedAlt, key.Meta);
    Assert.Equal(expectedShift, key.Shift);
  }

  [Theory]
  [InlineData("\u001b[5;5~", "pageup", true, false, false)]
  [InlineData("\u001b[3;3~", "alt+delete", false, true, false)]
  [InlineData("\u001b[4;2~", "end", false, false, true)]
  public void Feed_ParsesModifiersOnTildeKeys(
      string sequence,
      string expectedName,
      bool expectedCtrl,
      bool expectedAlt,
      bool expectedShift)
  {
    var parser = new StdinParser();

    var key = Assert.IsType<KeyEvent>(Assert.Single(parser.Feed(Encoding.ASCII.GetBytes(sequence))));

    Assert.Equal(expectedName, key.Name);
    Assert.Equal(expectedCtrl, key.Ctrl);
    Assert.Equal(expectedAlt, key.Alt);
    Assert.Equal(expectedAlt, key.Meta);
    Assert.Equal(expectedShift, key.Shift);
  }

  [Fact]
  public void Feed_ProducesTheSameModifierEventWhenSplitAtEveryByteBoundary()
  {
    var bytes = Encoding.ASCII.GetBytes("\u001b[1;5D");

    for (var split = 1; split < bytes.Length; split++)
    {
      var parser = new StdinParser();
      var first = parser.Feed(bytes.AsSpan(0, split));
      var second = parser.Feed(bytes.AsSpan(split));

      var key = Assert.IsType<KeyEvent>(Assert.Single(first.Concat(second)));
      Assert.Equal("left", key.Name);
      Assert.True(key.Ctrl);
      Assert.False(parser.HasPendingInput);
    }
  }

  [Fact]
  public void FlushPending_EmitsStandaloneEscapeAfterTimeout()
  {
    var parser = new StdinParser();

    Assert.Empty(parser.Feed([0x1b]));
    Assert.True(parser.HasPendingInput);

    var key = Assert.IsType<KeyEvent>(Assert.Single(parser.FlushPending()));

    Assert.Equal("escape", key.Name);
    Assert.False(parser.HasPendingInput);
  }

  [Fact]
  public void FlushPending_RecoversIncompleteCsiAsEscapeAndPrintableInput()
  {
    var parser = new StdinParser();

    Assert.Empty(parser.Feed([0x1b, (byte)'[', (byte)'1']));

    var keys = parser.FlushPending().OfType<KeyEvent>().ToArray();

    Assert.Equal(["escape", "[", "1"], keys.Select(key => key.Name));
    Assert.False(parser.HasPendingInput);
  }

  [Fact]
  public void Feed_ParsesAltLetterAfterEscapePrefix()
  {
    var parser = new StdinParser();

    var key = Assert.IsType<KeyEvent>(Assert.Single(parser.Feed(Encoding.ASCII.GetBytes("\u001ba"))));

    Assert.Equal("alt+a", key.Name);
    Assert.True(key.Alt);
    Assert.True(key.Meta);
    Assert.Equal('a', key.Char);
  }

  [Fact]
  public void Feed_PreservesUtf8CharacterSplitAcrossReads()
  {
    var parser = new StdinParser();
    var bytes = Encoding.UTF8.GetBytes("é");

    Assert.Empty(parser.Feed(bytes.AsSpan(0, 1)));
    Assert.True(parser.HasPendingInput);

    var key = Assert.IsType<KeyEvent>(Assert.Single(parser.Feed(bytes.AsSpan(1))));

    Assert.Equal("é", key.Name);
    Assert.Equal('é', key.Char);
    Assert.False(parser.HasPendingInput);
  }

  [Fact]
  public void Feed_RecoversFromOversizedIncompleteMouseSequence()
  {
    var parser = new StdinParser();
    var input = Encoding.ASCII.GetBytes("\u001b[<" + new string('1', 1024));

    var events = parser.Feed(input);

    Assert.False(parser.HasPendingInput);
    Assert.Equal(input.Length, events.Count());
    Assert.Equal("escape", Assert.IsType<KeyEvent>(events.First()).Name);
  }
}
