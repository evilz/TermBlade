using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a radio button control.
/// </summary>
public class RadioButtonRenderable : LabelRenderable
{
  /// <summary>Whether the radio button is checked.</summary>
  public bool IsChecked { get; set; }

  /// <summary>Text displayed after the radio button marker.</summary>
  public string Label { get; set; } = string.Empty;

  /// <summary>
  /// Initializes a new instance of the <see cref="RadioButtonRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public RadioButtonRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    Content = $"{(IsChecked ? "(o)" : "( )")} {Label}";
    base.RenderSelf(buffer, deltaTime);
  }
}
