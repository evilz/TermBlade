using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents segmented text.
/// </summary>
public sealed class SegmentedText : RenderableComponentBase<SegmentedTextRenderable>
{
  [Parameter] public IReadOnlyList<SegmentedTextSegment> Segments { get; set; } = [];
  [Parameter] public string BackgroundColor { get; set; } = "transparent";

  protected override SegmentedTextRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(SegmentedTextRenderable renderable)
  {
    renderable.Segments = Segments;
    renderable.BackgroundColor = BackgroundColor;
  }
}
