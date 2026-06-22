using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a hierarchical tree using the TermBlade tree view implementation.
/// </summary>
public class TreeRenderable : TreeViewRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="TreeRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public TreeRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
