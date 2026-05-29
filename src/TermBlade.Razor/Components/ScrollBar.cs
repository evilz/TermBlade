using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class ScrollBar : RenderableComponentBase<ScrollBarRenderable>
{
    [Parameter] public string Orientation { get; set; } = "vertical";
    [Parameter] public int ScrollPosition { get; set; }
    [Parameter] public int ScrollSize { get; set; } = 100;
    [Parameter] public int ViewportSize { get; set; } = 10;
    [Parameter] public string? TrackColor { get; set; } = "#333333";
    [Parameter] public string? ThumbColor { get; set; } = "#888888";

    protected override ScrollBarRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

    protected override void ApplyParameters(ScrollBarRenderable renderable)
    {
        renderable.Orientation = Orientation;
        renderable.ScrollPosition = ScrollPosition;
        renderable.ScrollSize = ScrollSize;
        renderable.ViewportSize = ViewportSize;
        renderable.TrackColor = TrackColor;
        renderable.ThumbColor = ThumbColor;
    }
}
