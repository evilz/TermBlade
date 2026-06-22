using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a combobox prompt.
/// </summary>
public sealed class ComboBox : SelectionPromptBase<ComboBoxRenderable>
{
  protected override ComboBoxRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
