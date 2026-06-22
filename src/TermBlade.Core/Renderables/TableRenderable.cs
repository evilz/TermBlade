using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders rows and columns as a compact text table.
/// </summary>
public class TableRenderable : Renderable
{
  /// <summary>Column headings displayed at the top of the table.</summary>
  public IReadOnlyList<string> Columns { get; set; } = [];

  /// <summary>Rows displayed in the table body.</summary>
  public IReadOnlyList<IReadOnlyList<string>> Rows { get; set; } = [];

  /// <summary>Optional foreground color as a CSS color string.</summary>
  public string? Fg { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="TableRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public TableRenderable(CliRenderer? renderer) : base(renderer)
  {
  }

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    var w = ComputedWidth;
    var h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = LabelRenderable.ParseColor(Fg, Rgba.FromInts(255, 255, 255));
    var bg = Rgba.FromValues(0, 0, 0, 0);
    var widths = BuildWidths(w);
    var row = 0;

    if (Columns.Count > 0 && row < h)
      buffer.DrawText(ScreenX, ScreenY + row++, FormatRow(Columns, widths), fg, bg, TextAttributes.Bold);

    foreach (var values in Rows)
    {
      if (row >= h) break;
      buffer.DrawText(ScreenX, ScreenY + row++, FormatRow(values, widths), fg, bg);
    }
  }

  private int[] BuildWidths(int availableWidth)
  {
    var count = Math.Max(1, Math.Max(Columns.Count, Rows.Select(r => r.Count).DefaultIfEmpty(0).Max()));
    var width = Math.Max(1, (availableWidth - (count - 1) * 3) / count);
    return Enumerable.Repeat(width, count).ToArray();
  }

  private static string FormatRow(IReadOnlyList<string> cells, IReadOnlyList<int> widths)
  {
    var formatted = new string[widths.Count];
    for (var i = 0; i < widths.Count; i++)
    {
      var cell = i < cells.Count ? cells[i] : string.Empty;
      formatted[i] = LabelRenderable.TrimToWidth(cell, widths[i]).PadRight(widths[i]);
    }
    return string.Join(" | ", formatted);
  }
}
