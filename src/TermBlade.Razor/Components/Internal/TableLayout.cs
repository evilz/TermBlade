namespace TermBlade.Razor.Components;

internal static class TableLayout
{
  /// <summary>
  /// Create.
  /// </summary>
  public static TableLayoutResult Create(
      IReadOnlyList<string> headers,
      IReadOnlyList<IReadOnlyList<string>> rows,
      int rowOffset,
      int columnOffset,
      int visibleRowCount,
      int minColumnWidth,
      int maxColumnWidth,
      int maxVisibleWidth)
  {
    ArgumentNullException.ThrowIfNull(headers);
    ArgumentNullException.ThrowIfNull(rows);

    minColumnWidth = Math.Max(1, minColumnWidth);
    maxColumnWidth = Math.Max(minColumnWidth, maxColumnWidth);
    maxVisibleWidth = Math.Max(1, maxVisibleWidth);

    var columns = BuildColumns(headers, rows, Math.Max(0, columnOffset), minColumnWidth, maxColumnWidth, maxVisibleWidth).ToArray();
    var separator = new string('─', Math.Min(maxVisibleWidth, Math.Max(0, columns.Sum(column => column.Width + 1) + 1)));
    var visibleRows = rows
        .Skip(Math.Max(0, rowOffset))
        .Take(Math.Max(0, visibleRowCount))
        .Select(row => FormatRow(row, columns))
        .ToArray();
    var header = FormatRow(headers, columns);

    return new TableLayoutResult(header, separator, visibleRows);
  }

  private static IEnumerable<TableLayoutColumn> BuildColumns(
      IReadOnlyList<string> headers,
      IReadOnlyList<IReadOnlyList<string>> rows,
      int columnOffset,
      int minColumnWidth,
      int maxColumnWidth,
      int maxVisibleWidth)
  {
    var columnCount = Math.Max(headers.Count, rows.Count == 0 ? 0 : rows.Max(row => row.Count));
    var used = 1;
    for (var column = columnOffset; column < columnCount; column++)
    {
      var headerWidth = GetCell(headers, column).Length;
      var dataWidth = rows.Take(200).Select(row => GetCell(row, column).Length).DefaultIfEmpty(0).Max();
      var width = Math.Clamp(Math.Max(headerWidth, dataWidth), minColumnWidth, maxColumnWidth);
      if (used + width + 1 > maxVisibleWidth)
        yield break;

      used += width + 1;
      yield return new TableLayoutColumn(column, width);
    }
  }

  private static string FormatRow(IReadOnlyList<string> row, IReadOnlyList<TableLayoutColumn> columns)
  {
    var cells = columns.Select(column => Trim(GetCell(row, column.Index).ReplaceLineEndings(" "), column.Width).PadRight(column.Width));
    return "│" + string.Join("│", cells) + "│";
  }

  private static string GetCell(IReadOnlyList<string> row, int index) => index < row.Count ? row[index] : string.Empty;

  private static string Trim(string value, int width) => value.Length <= width ? value : value[..Math.Max(0, width - 1)] + "…";
}

