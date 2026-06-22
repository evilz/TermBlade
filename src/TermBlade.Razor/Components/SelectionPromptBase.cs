using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Base class for selection prompt wrappers.
/// </summary>
public abstract class SelectionPromptBase<TRenderable> : RenderableComponentBase<TRenderable> where TRenderable : SelectionPromptRenderable
{
  [Parameter] public List<SelectOption> Options { get; set; } = [];
  [Parameter] public int SelectedIndex { get; set; }

  protected override void ApplyParameters(TRenderable renderable)
  {
    renderable.Options = Options;
    renderable.SelectedIndex = SelectedIndex;
  }
}
