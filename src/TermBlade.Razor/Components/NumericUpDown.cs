using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a numeric up/down control.
/// </summary>
public sealed class NumericUpDown : RenderableComponentBase<NumericUpDownRenderable>
{
  [Parameter] public double Value { get; set; }
  [Parameter] public double Step { get; set; } = 1;

  protected override NumericUpDownRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(NumericUpDownRenderable renderable)
  {
    renderable.Value = Value;
    renderable.Step = Step;
  }
}
