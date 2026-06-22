using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders large figlet-style text using TermBlade's ASCII font renderer.
/// </summary>
public class FigletTextRenderable : ASCIIFontRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="FigletTextRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public FigletTextRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
