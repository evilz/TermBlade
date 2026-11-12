using TermBlade.CsvViewer;
using TermBlade.Razor.Components;

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

  [Theory]
  [InlineData("\"\"")]
  [InlineData("\"\"\n")]
  [InlineData("\"\"\r\n")]
  public void Parse_PreservesFinalQuotedEmptyFieldWithoutAddingBlankRow(string content)
  {
    var document = CsvDocument.Parse(content, ',', hasHeader: false);

    Assert.Single(document.Rows);
    Assert.Single(document.Rows[0]);
    Assert.Equal(string.Empty, document.Rows[0][0]);
    Assert.Equal(1, document.ColumnCount);
  }

  [Fact]
  public void Parse_PreservesQuotedEmptyHeader()
  {
    var document = CsvDocument.Parse("\"\"", ',', hasHeader: true);

    Assert.Equal([""], document.Headers);
    Assert.Empty(document.Rows);
    Assert.Equal(1, document.ColumnCount);
  }

  [Fact]
  public void Parse_PreservesTrailingEmptyField()
  {
    var document = CsvDocument.Parse("name,", ',', hasHeader: false);

    Assert.Equal(["name", ""], document.Rows[0]);
    Assert.Equal(2, document.ColumnCount);
  }

  [Fact]
  public void Parse_PreservesBlankRecordsWithoutAddingOneAfterFinalNewline()
  {
    var document = CsvDocument.Parse("first\n\n", ',', hasHeader: false);

    Assert.Equal(2, document.Rows.Count);
    Assert.Equal(["first"], document.Rows[0]);
    Assert.Equal([""], document.Rows[1]);
  }

  [Fact]
  public void Options_ParseSupportsTabDelimiterAlias()
  {
    var options = CsvViewerOptions.Parse(["--delimiter", "tab", "data.tsv"]);

    Assert.Equal('\t', options.Delimiter);
    Assert.Equal("data.tsv", options.FilePath);
  }

  [Fact]
  public void Options_ParseRejectsUnknownOptions()
  {
    var ex = Assert.Throws<ArgumentException>(() => CsvViewerOptions.Parse(["--bad", "data.csv"]));

    Assert.Contains("Unknown option", ex.Message);
  }

  [Fact]
  public void TableLayout_TruncatesColumnsAndAppliesOffsets()
  {
    var rows = new[]
    {
        new[] { "Ada Lovelace", "Mathematician", "1843" },
        new[] { "Grace Hopper", "Computer scientist", "1952" }
    };

    var layout = TableLayout.Create(["Name", "Role", "Year"], rows, rowOffset: 1, columnOffset: 1, visibleRowCount: 1, minColumnWidth: 4, maxColumnWidth: 10, maxVisibleWidth: 18);

    Assert.Equal("│Role      │Year│", layout.Header);
    Assert.Equal(["│Computer …│1952│"], layout.Rows);
  }
}
