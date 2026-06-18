using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using TermBlade.Core.Renderables;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents container renderable component base.
/// </summary>
public abstract class ContainerRenderableComponentBase<TRenderable> : RenderableComponentBase<TRenderable>, IRenderableParent where TRenderable : Renderable
{
  private readonly HashSet<string> _childIds = [];

  [Parameter] public RenderFragment? ChildContent { get; set; }

  protected override void BuildRenderTree(RenderTreeBuilder builder)
  {
    if (ChildContent == null)
      return;

    builder.OpenComponent<CascadingValue<IRenderableParent>>(0);
    builder.AddAttribute(1, nameof(CascadingValue<IRenderableParent>.Value), this);
    builder.AddAttribute(2, nameof(CascadingValue<IRenderableParent>.IsFixed), true);
    builder.AddAttribute(3, nameof(CascadingValue<IRenderableParent>.ChildContent), (RenderFragment)(childBuilder => childBuilder.AddContent(4, ChildContent)));
    builder.CloseComponent();
  }

  /// <summary>
  /// Add child.
  /// </summary>
  /// <param name="child">The child value.</param>
  public void AddChild(Renderable child)
  {
    if (_childIds.Add(child.Id))
      Renderable.Add(child);
  }

  /// <summary>
  /// Remove child.
  /// </summary>
  /// <param name="child">The child value.</param>
  public void RemoveChild(Renderable child)
  {
    _childIds.Remove(child.Id);
    Renderable.Remove(child.Id);
  }
}
