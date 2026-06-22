using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders an on/off toggle switch.
/// </summary>
public class ToggleSwitchRenderable : LabelRenderable
{
  /// <summary>Whether the toggle switch is on.</summary>
  public bool IsOn { get; set; }

  /// <summary>Text displayed after the switch.</summary>
  public string Label { get; set; } = string.Empty;

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    Content = $"{(IsOn ? "[ON ]" : "[OFF]")} {Label}";
    base.RenderSelf(buffer, deltaTime);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ToggleSwitchRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public ToggleSwitchRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }
}
