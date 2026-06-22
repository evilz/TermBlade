using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a checkbox control.
/// </summary>
public class CheckBoxRenderable : LabelRenderable
{
  /// <summary>Whether the checkbox is checked.</summary>
  public bool IsChecked { get; set; }

  /// <summary>Text displayed after the checkbox marker.</summary>
  public string Label { get; set; } = string.Empty;

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    Content = $"{(IsChecked ? "[x]" : "[ ]")} {Label}";
    base.RenderSelf(buffer, deltaTime);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="CheckBoxRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public CheckBoxRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }
}
