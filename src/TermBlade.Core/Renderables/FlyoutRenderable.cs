using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a floating flyout panel.
/// </summary>
public class FlyoutRenderable : PanelRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="FlyoutRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public FlyoutRenderable(CliRenderer? renderer) : base(renderer)
  {
    ZIndex = 100;
  }
}
