using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a numeric up/down control.
/// </summary>
public class NumericUpDownRenderable : LabelRenderable
{
  /// <summary>Current numeric value.</summary>
  public double Value { get; set; }

  /// <summary>Amount added or removed by keyboard interaction.</summary>
  public double Step { get; set; } = 1;

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    Content = $"[-] {Value} [+]";
    base.RenderSelf(buffer, deltaTime);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="NumericUpDownRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public NumericUpDownRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }
}
