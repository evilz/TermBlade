using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a radio button.
/// </summary>
public sealed class RadioButton : RenderableComponentBase<RadioButtonRenderable>
{
  [Parameter] public bool IsChecked { get; set; }
  [Parameter] public string Label { get; set; } = string.Empty;

  protected override RadioButtonRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(RadioButtonRenderable renderable)
  {
    renderable.IsChecked = IsChecked;
    renderable.Label = Label;
  }
}
