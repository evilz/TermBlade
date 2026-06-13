using TermBlade.CsvViewer;

namespace TermBlade.Tests;

public class CsvViewerTests
{
  [Fact]
  public void Parse_HandlesQuotedFieldsAndEscapedQuotes()
  {
    var document = CsvDocument.Parse("Name,Note\nAda,\"Hello, CSV\"\nGrace,\"She said \"\"hi\"\"\"", hasHeader: true);

    Assert.Equal(',', document.Delimiter);
    Assert.Equal(["Name", "Note"], document.Headers);
    Assert.Equal("Hello, CSV", document.Rows[0][1]);
    Assert.Equal("She said \"hi\"", document.Rows[1][1]);
  }

  [Fact]
  public void Parse_DetectsSemicolonDelimiter()
  {
    var document = CsvDocument.Parse("Name;Score\nAda;10", hasHeader: true);

    Assert.Equal(';', document.Delimiter);
    Assert.Equal("Score", document.Headers[1]);
    Assert.Equal("10", document.Rows[0][1]);
  }

  [Fact]
  public void Parse_GeneratesHeaders_WhenNoHeaderIsSelected()
  {
    var document = CsvDocument.Parse("Ada,10\nGrace,9", hasHeader: false);

    Assert.Equal(["Column 1", "Column 2"], document.Headers);
    Assert.Equal(2, document.Rows.Count);
  }
}
