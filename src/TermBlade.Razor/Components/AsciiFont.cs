using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class AsciiFont : RenderableComponentBase<ASCIIFontRenderable>
{
    [Parameter] public string Text { get; set; } = string.Empty;
    [Parameter] public string Font { get; set; } = "normal";
    [Parameter] public string? Color { get; set; }
    [Parameter] public string? BackgroundColor { get; set; }
    [Parameter] public string SelectionBg { get; set; } = "#4a5568";
    [Parameter] public string SelectionFg { get; set; } = "#ffffff";

    protected override ASCIIFontRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

    protected override void ApplyParameters(ASCIIFontRenderable renderable)
    {
        renderable.Text = Text;
        renderable.Font = Font;
        renderable.Color = Color;
        renderable.BackgroundColor = BackgroundColor;
        renderable.SelectionBg = SelectionBg;
        renderable.SelectionFg = SelectionFg;
    }
}
