using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a single-selection prompt.
/// </summary>
public sealed class SelectionPrompt : RenderableComponentBase<SelectionPromptRenderable>
{
  [Parameter] public List<SelectOption> Options { get; set; } = [];
  [Parameter] public int SelectedIndex { get; set; }

  protected override SelectionPromptRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(SelectionPromptRenderable renderable)
  {
    renderable.Options = Options;
    renderable.SelectedIndex = SelectedIndex;
  }
}
