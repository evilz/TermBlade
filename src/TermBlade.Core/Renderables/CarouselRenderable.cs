using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders one selected item from a carousel of text items.
/// </summary>
public class CarouselRenderable : LabelRenderable
{
  /// <summary>Items available in the carousel.</summary>
  public IReadOnlyList<string> Items { get; set; } = [];

  /// <summary>Index of the current carousel item.</summary>
  public int SelectedIndex { get; set; }

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    if (Items.Count > 0)
      Content = Items[Math.Clamp(SelectedIndex, 0, Items.Count - 1)];
    base.RenderSelf(buffer, deltaTime);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="CarouselRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public CarouselRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }
}
