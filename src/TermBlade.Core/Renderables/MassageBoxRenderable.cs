using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Compatibility alias for the misspelled message box name in older requests.
/// </summary>
public class MassageBoxRenderable : MessageBoxRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MassageBoxRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public MassageBoxRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
