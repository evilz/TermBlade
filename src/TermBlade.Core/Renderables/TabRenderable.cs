using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a tab item label.
/// </summary>
public class TabRenderable : LabelRenderable
{
  /// <summary>Whether the tab is selected.</summary>
  public bool IsSelected { get; set; }

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    Content = IsSelected ? $"[{Content}]" : $" {Content} ";
    base.RenderSelf(buffer, deltaTime);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TabRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public TabRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
