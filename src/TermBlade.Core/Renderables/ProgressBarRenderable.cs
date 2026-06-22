using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a progress bar.
/// </summary>
public class ProgressBarRenderable : LabelRenderable
{
  /// <summary>Current progress value between zero and one.</summary>
  public double Value { get; set; }

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    var w = Math.Max(0, ComputedWidth - 2);
    var filled = (int)Math.Round(Math.Clamp(Value, 0, 1) * w);
    Content = "[" + new string('#', filled) + new string('-', Math.Max(0, w - filled)) + "]";
    base.RenderSelf(buffer, deltaTime);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ProgressBarRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ProgressBarRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
