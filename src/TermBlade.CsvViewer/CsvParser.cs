using System.Text;

namespace TermBlade.CsvViewer;

internal static class CsvParser
{
  public static IReadOnlyList<IReadOnlyList<string>> Parse(string content, char delimiter)
  {
    var rows = new List<IReadOnlyList<string>>();
    var row = new List<string>();
    var field = new StringBuilder();
    var inQuotes = false;

    for (var i = 0; i < content.Length; i++)
    {
      var current = content[i];
      if (current == '"')
      {
        if (inQuotes && i + 1 < content.Length && content[i + 1] == '"')
        {
          field.Append('"');
          i++;
        }
        else
        {
          inQuotes = !inQuotes;
        }
      }
      else if (current == delimiter && !inQuotes)
      {
        row.Add(field.ToString());
        field.Clear();
      }
      else if ((current == '\r' || current == '\n') && !inQuotes)
      {
        if (current == '\r' && i + 1 < content.Length && content[i + 1] == '\n')
        {
          i++;
        }

        row.Add(field.ToString());
        field.Clear();
        rows.Add(row.ToArray());
        row.Clear();
      }
      else
      {
        field.Append(current);
      }
    }

    if (field.Length > 0 || row.Count > 0)
    {
      row.Add(field.ToString());
      rows.Add(row.ToArray());
    }

    return rows;
  }
}
