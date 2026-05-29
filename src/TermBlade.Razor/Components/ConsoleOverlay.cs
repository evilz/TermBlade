using TermBlade.Core.Console;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class ConsoleOverlay : RenderableComponentBase<TermBlade.Core.Console.ConsoleOverlay>
{
    [Parameter] public OverlayPosition Position { get; set; } = OverlayPosition.BottomRight;
    [Parameter] public int MaxLines { get; set; } = 10;
    [Parameter] public int MaxWidth { get; set; } = 60;
    [Parameter] public IReadOnlyList<string>? Lines { get; set; }

    protected override TermBlade.Core.Console.ConsoleOverlay CreateRenderable(CliRenderer renderer) => new(renderer);

    protected override void ApplyParameters(TermBlade.Core.Console.ConsoleOverlay renderable)
    {
        renderable.Position = Position;
        renderable.MaxLines = MaxLines;
        renderable.MaxWidth = MaxWidth;
        renderable.ZIndex = ZIndex ?? 1000;
        if (Lines == null)
            return;

        renderable.Clear();
        foreach (var line in Lines)
            renderable.AddLine(line);
    }
}
