using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders figlet-style text.
/// </summary>
public sealed class FigletText : RenderableComponentBase<FigletTextRenderable>
{
  [Parameter] public string Text { get; set; } = string.Empty;
  [Parameter] public string? Fg { get; set; }

  protected override FigletTextRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(FigletTextRenderable renderable)
  {
    renderable.Text = Text;
    renderable.Color = Fg;
  }
}
