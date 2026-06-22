using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Applies padding around child renderables.
/// </summary>
public class PadderRenderable : BoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PadderRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public PadderRenderable(CliRenderer? renderer) : base(renderer, new BoxOptions { PaddingTop = 1, PaddingRight = 1, PaddingBottom = 1, PaddingLeft = 1, ShouldFill = false })
  {
  }
}
