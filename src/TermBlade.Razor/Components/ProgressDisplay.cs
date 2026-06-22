using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a live progress display.
/// </summary>
public sealed class ProgressDisplay : RenderableComponentBase<ProgressDisplayRenderable>
{
  [Parameter] public double Value { get; set; }

  protected override ProgressDisplayRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(ProgressDisplayRenderable renderable) => renderable.Value = Value;
}
