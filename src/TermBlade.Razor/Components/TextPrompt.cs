using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a text prompt.
/// </summary>
public sealed class TextPrompt : RenderableComponentBase<TextPromptRenderable>
{
  [Parameter] public string Value { get; set; } = string.Empty;
  [Parameter] public string Placeholder { get; set; } = string.Empty;

  protected override TextPromptRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(TextPromptRenderable renderable)
  {
    renderable.Value = Value;
    renderable.Placeholder = Placeholder;
  }
}
