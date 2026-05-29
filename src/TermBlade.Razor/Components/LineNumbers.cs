using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class LineNumbers : RenderableComponentBase<LineNumbersRenderable>
{
    [Parameter] public int LineCount { get; set; }
    [Parameter] public int StartLine { get; set; } = 1;
    [Parameter] public string? Fg { get; set; } = "#666666";
    [Parameter] public string? Bg { get; set; }

    protected override LineNumbersRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

    protected override void ApplyParameters(LineNumbersRenderable renderable)
    {
        renderable.LineCount = LineCount;
        renderable.StartLine = StartLine;
        renderable.Fg = Fg;
        renderable.Bg = Bg;
    }
}
