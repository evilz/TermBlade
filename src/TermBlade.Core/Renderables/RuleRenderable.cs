using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Buffer;
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
    var title = CellBuffer.TruncateToCellWidth($" {originalContent} ", Math.Max(0, ComputedWidth - 2));
    int titleWidth = CellBuffer.GetTextWidth(title.AsSpan());
    if (!string.IsNullOrEmpty(originalContent) && titleWidth + 2 < ComputedWidth)
    {
      int leftWidth = (ComputedWidth - titleWidth) / 2;
      int rightWidth = ComputedWidth - leftWidth - titleWidth;
      line = new string(RuleCharacter, leftWidth) + title + new string(RuleCharacter, rightWidth);
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
