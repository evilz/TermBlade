using TermBlade.Core.Renderables;

namespace TermBlade.Razor.Components;

public interface IRenderableParent
{
  void AddChild(Renderable child);
  void RemoveChild(Renderable child);
}
