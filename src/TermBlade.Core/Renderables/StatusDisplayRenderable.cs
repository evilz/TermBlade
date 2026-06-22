using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a live status line.
/// </summary>
public class StatusDisplayRenderable : LabelRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="StatusDisplayRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public StatusDisplayRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
