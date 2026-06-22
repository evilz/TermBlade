using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a modal message box.
/// </summary>
public sealed class MessageBox : ContainerAdapterComponentBase<MessageBoxRenderable>
{
  protected override MessageBoxRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
