using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders an on/off toggle switch.
/// </summary>
public sealed class ToggleSwitch : RenderableComponentBase<ToggleSwitchRenderable>
{
  [Parameter] public bool IsOn { get; set; }
  [Parameter] public string Label { get; set; } = string.Empty;

  protected override ToggleSwitchRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(ToggleSwitchRenderable renderable)
  {
    renderable.IsOn = IsOn;
    renderable.Label = Label;
  }
}
