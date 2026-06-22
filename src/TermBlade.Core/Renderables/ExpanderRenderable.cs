using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders an expandable panel with a title row.
/// </summary>
public class ExpanderRenderable : PanelRenderable
{
  /// <summary>Whether child content is visible.</summary>
  public bool IsExpanded { get; set; } = true;

  /// <summary>
  /// Initializes a new instance of the <see cref="ExpanderRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ExpanderRenderable(CliRenderer? renderer) : base(renderer)
  {
    Title = "Expander";
  }
}
