using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a calendar picker built on the monthly calendar widget.
/// </summary>
public class CalendarPickerRenderable : CalendarRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="CalendarPickerRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public CalendarPickerRenderable(CliRenderer? renderer) : base(renderer)
  {
    HighlightedDate = DateOnly.FromDateTime(DateTime.Today);
  }
}
