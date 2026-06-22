using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders an autocomplete prompt.
/// </summary>
public sealed class AutoComplete : SelectionPromptBase<AutoCompleteRenderable>
{
  protected override AutoCompleteRenderable CreateRenderable(CliRenderer renderer) => new(renderer);
}
