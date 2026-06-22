using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a checkbox.
/// </summary>
public sealed class CheckBox : RenderableComponentBase<CheckBoxRenderable>
{
  [Parameter] public bool IsChecked { get; set; }
  [Parameter] public string Label { get; set; } = string.Empty;

  protected override CheckBoxRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(CheckBoxRenderable renderable)
  {
    renderable.IsChecked = IsChecked;
    renderable.Label = Label;
  }
}
