using TermBlade.Core.Renderables;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents irenderable parent.
/// </summary>
public interface IRenderableParent
{
  void AddChild(Renderable child);
  void RemoveChild(Renderable child);
}
