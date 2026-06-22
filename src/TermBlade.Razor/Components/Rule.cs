using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders a horizontal rule.
/// </summary>
public sealed class Rule : LabelRenderableComponentBase<RuleRenderable>
{
  [Parameter] public char RuleCharacter { get; set; } = '-';

  protected override RuleRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(RuleRenderable renderable)
  {
    base.ApplyParameters(renderable);
    renderable.RuleCharacter = RuleCharacter;
  }
}
