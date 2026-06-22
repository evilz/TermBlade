using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders proportional category values as a breakdown chart.
/// </summary>
public class BreakdownChartRenderable : BarChartRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BreakdownChartRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public BreakdownChartRenderable(CliRenderer? renderer) : base(renderer)
  {
    Orientation = "horizontal";
  }
}
