namespace TermBlade.CsvViewer;

/// <summary>
/// Represents a parsed delimited text document with optional headers.
/// </summary>
public sealed class CsvDocument
{
  private readonly IReadOnlyList<IReadOnlyList<string>> _dataRows;

  private CsvDocument(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> dataRows, char delimiter)
  {
    Headers = headers;
    _dataRows = dataRows;
    Delimiter = delimiter;
    ColumnCount = Math.Max(headers.Count, dataRows.Count == 0 ? 0 : dataRows.Max(row => row.Count));
  }

  /// <summary>Gets the column header labels.</summary>
  public IReadOnlyList<string> Headers { get; }

  /// <summary>Gets the parsed data rows.</summary>
  public IReadOnlyList<IReadOnlyList<string>> Rows => _dataRows;

  /// <summary>Gets the delimiter used to parse the document.</summary>
  public char Delimiter { get; }

  /// <summary>Gets the maximum number of columns across headers and rows.</summary>
  public int ColumnCount { get; }

  /// <summary>Loads and parses a CSV document from a file path.</summary>
  /// <param name="path">The CSV file path.</param>
  /// <param name="delimiter">Optional explicit delimiter. When omitted, the delimiter is detected from the first non-empty line.</param>
  /// <param name="hasHeader">Whether the first row contains column names.</param>
  /// <returns>The parsed CSV document.</returns>
  public static async Task<CsvDocument> LoadAsync(string path, char? delimiter, bool hasHeader)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(path);

    var content = await File.ReadAllTextAsync(path).ConfigureAwait(false);
    return Parse(content, delimiter, hasHeader);
  }

  /// <summary>Parses delimited text into a CSV document.</summary>
  /// <param name="content">The delimited text content.</param>
  /// <param name="delimiter">Optional explicit delimiter. When omitted, a delimiter is detected.</param>
  /// <param name="hasHeader">Whether the first row contains column names.</param>
  /// <returns>The parsed CSV document.</returns>
  public static CsvDocument Parse(string content, char? delimiter = null, bool hasHeader = true)
  {
    ArgumentNullException.ThrowIfNull(content);

    var actualDelimiter = delimiter ?? DetectDelimiter(content);
    var rows = CsvParser.Parse(content, actualDelimiter);
    var headers = hasHeader && rows.Count > 0
        ? rows[0]
        : CreateDefaultHeaders(rows.Count == 0 ? 0 : rows.Max(row => row.Count));
    var dataRows = hasHeader && rows.Count > 0 ? rows.Skip(1).ToArray() : rows;

    return new CsvDocument(headers, dataRows, actualDelimiter);
  }

  private static IReadOnlyList<string> CreateDefaultHeaders(int count)
  {
    var headers = new string[count];
    for (var i = 0; i < count; i++)
    {
      headers[i] = $"Column {i + 1}";
    }

    return headers;
  }

  private static char DetectDelimiter(string content)
  {
    var firstLine = content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
    var candidates = new[] { ',', ';', '\t', '|' };
    return candidates.OrderByDescending(candidate => firstLine.Count(character => character == candidate)).First();
  }
}
