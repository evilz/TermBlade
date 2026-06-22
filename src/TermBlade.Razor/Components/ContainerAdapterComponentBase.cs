using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Base class for container components that expose child content.
/// </summary>
public abstract class ContainerAdapterComponentBase<TRenderable> : ContainerRenderableComponentBase<TRenderable> where TRenderable : Renderable
{
  protected override void ApplyParameters(TRenderable renderable)
  {
  }
}
