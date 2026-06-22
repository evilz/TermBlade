using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders a horizontal rule, optionally with a centered title.
/// </summary>
public class RuleRenderable : LabelRenderable
{
  /// <summary>Line character used to draw the rule.</summary>
  public char RuleCharacter { get; set; } = '-';

  /// <inheritdoc />
  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    if (ComputedWidth <= 0 || ComputedHeight <= 0) return;
    var originalContent = Content;
    var line = new string(RuleCharacter, ComputedWidth);
    if (!string.IsNullOrEmpty(originalContent) && originalContent.Length + 2 < ComputedWidth)
    {
      var title = $" {originalContent} ";
      var start = (ComputedWidth - title.Length) / 2;
      line = line.Remove(start, title.Length).Insert(start, title);
    }
    Content = line;
    base.RenderSelf(buffer, deltaTime);
    Content = originalContent;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="RuleRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public RuleRenderable(CliRenderer? renderer) : base(renderer)
  {
  }
}
