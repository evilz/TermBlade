using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

public class RootRenderable : Renderable
{
  public RootRenderable(CliRenderer renderer) : base(renderer)
  {
    Id = "root";
    LayoutNode.Width = LayoutDimension.Fixed(renderer.TerminalWidth);
    LayoutNode.Height = LayoutDimension.Fixed(renderer.TerminalHeight);
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime) { }

  public void UpdateSize(int width, int height)
  {
    LayoutNode.Width = LayoutDimension.Fixed(width);
    LayoutNode.Height = LayoutDimension.Fixed(height);
  }
}
