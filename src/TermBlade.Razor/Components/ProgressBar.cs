using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a progress bar.
/// </summary>
public sealed class ProgressBar : RenderableComponentBase<ProgressBarRenderable>
{
  [Parameter] public double Value { get; set; }

  protected override ProgressBarRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(ProgressBarRenderable renderable) => renderable.Value = Value;
}
