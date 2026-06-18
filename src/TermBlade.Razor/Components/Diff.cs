using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents diff.
/// </summary>
public sealed class Diff : RenderableComponentBase<DiffRenderable>
{
  [Parameter] public string OldText { get; set; } = string.Empty;
  [Parameter] public string NewText { get; set; } = string.Empty;
  [Parameter] public bool ShowLineNumbers { get; set; } = true;

  protected override DiffRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(DiffRenderable renderable)
  {
    renderable.OldText = OldText;
    renderable.NewText = NewText;
    renderable.ShowLineNumbers = ShowLineNumbers;
  }
}
