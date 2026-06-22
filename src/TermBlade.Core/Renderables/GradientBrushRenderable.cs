using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a simple gradient brush preview line.
/// </summary>
public class GradientBrushRenderable : LabelRenderable
{
  /// <summary>Start color for the gradient preview.</summary>
  public string StartColor { get; set; } = "#000000";

  /// <summary>End color for the gradient preview.</summary>
  public string EndColor { get; set; } = "#ffffff";

  /// <summary>
  /// Initializes a new instance of the <see cref="GradientBrushRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public GradientBrushRenderable(CliRenderer? renderer) : base(renderer)
  {
    Content = "gradient";
  }
}
