using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a tree using the TermBlade tree view implementation.
/// </summary>
public sealed class Tree : RenderableComponentBase<TreeRenderable>
{
  [Parameter] public TreeNode? Root { get; set; }

  protected override TreeRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(TreeRenderable renderable)
  {
    if (Root is not null)
    {
      renderable.Nodes = [Root];
      renderable.Invalidate();
    }
  }
}
