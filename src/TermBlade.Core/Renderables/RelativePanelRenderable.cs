using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a relative-positioned panel.
/// </summary>
public class RelativePanelRenderable : PanelRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RelativePanelRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public RelativePanelRenderable(CliRenderer? renderer) : base(renderer)
  {
    Position = "relative";
  }
}
