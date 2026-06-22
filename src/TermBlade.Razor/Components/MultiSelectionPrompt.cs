using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a multi-selection prompt.
/// </summary>
public sealed class MultiSelectionPrompt : RenderableComponentBase<MultiSelectionPromptRenderable>
{
  [Parameter] public List<SelectOption> Options { get; set; } = [];
  [Parameter] public HashSet<int> SelectedIndices { get; set; } = [];

  protected override MultiSelectionPromptRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(MultiSelectionPromptRenderable renderable)
  {
    renderable.Options = Options;
    renderable.SelectedIndices = SelectedIndices;
  }
}
