using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents root renderable.
/// </summary>
public class RootRenderable : Renderable
{
  /// <summary>
  /// Root renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public RootRenderable(CliRenderer renderer) : base(renderer)
  {
    Id = "root";
    LayoutNode.Width = LayoutDimension.Fixed(renderer.TerminalWidth);
    LayoutNode.Height = LayoutDimension.Fixed(renderer.TerminalHeight);
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime) { }

  /// <summary>
  /// Update size.
  /// </summary>
  /// <param name="width">The width value.</param>
  /// <param name="height">The height value.</param>
  public void UpdateSize(int width, int height)
  {
    LayoutNode.Width = LayoutDimension.Fixed(width);
    LayoutNode.Height = LayoutDimension.Fixed(height);
  }
}
