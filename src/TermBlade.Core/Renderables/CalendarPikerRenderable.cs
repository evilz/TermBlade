using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Compatibility alias for the misspelled calendar picker name in older requests.
/// </summary>
public class CalendarPikerRenderable : CalendarPickerRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="CalendarPikerRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public CalendarPikerRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
