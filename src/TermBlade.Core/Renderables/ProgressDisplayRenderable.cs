using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders live progress status with a progress bar.
/// </summary>
public class ProgressDisplayRenderable : ProgressBarRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ProgressDisplayRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ProgressDisplayRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
