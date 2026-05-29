using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class Box : ContainerRenderableComponentBase<BoxRenderable>
{
  [Parameter] public string BackgroundColor { get; set; } = "transparent";
  [Parameter] public bool Border { get; set; }
  [Parameter] public string BorderStyle { get; set; } = "single";
  [Parameter] public string BorderColor { get; set; } = "#ffffff";
  [Parameter] public string? FocusedBorderColor { get; set; }
  [Parameter] public bool ShouldFill { get; set; } = true;
  [Parameter] public string? Title { get; set; }
  [Parameter] public string TitleAlignment { get; set; } = "left";
  [Parameter] public string? BottomTitle { get; set; }
  [Parameter] public string BottomTitleAlignment { get; set; } = "left";

  protected override BoxRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(BoxRenderable renderable)
  {
    renderable.BackgroundColor = BackgroundColor;
    renderable.Border = Border;
    renderable.BorderStyle = BorderStyle;
    renderable.BorderColor = BorderColor;
    renderable.FocusedBorderColor = FocusedBorderColor;
    renderable.ShouldFill = ShouldFill;
    renderable.Title = Title;
    renderable.TitleAlignment = TitleAlignment;
    renderable.BottomTitle = BottomTitle;
    renderable.BottomTitleAlignment = BottomTitleAlignment;
    renderable.Focusable = Focusable ?? FocusedBorderColor != null;
  }
}
