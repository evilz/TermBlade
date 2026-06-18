using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents scroll box.
/// </summary>
public sealed class ScrollBox : ContainerRenderableComponentBase<ScrollBoxRenderable>
{
  [Parameter] public int ScrollX { get; set; }
  [Parameter] public int ScrollY { get; set; }
  [Parameter] public bool StickyScroll { get; set; }
  [Parameter] public bool ShowVerticalScrollbar { get; set; } = true;
  [Parameter] public bool ShowHorizontalScrollbar { get; set; }
  [Parameter] public bool Border { get; set; } = true;
  [Parameter] public string BorderColor { get; set; } = "#7aa2f7";
  [Parameter] public string BackgroundColor { get; set; } = "#1a1b26";
  [Parameter] public string TrackColor { get; set; } = "#414868";
  [Parameter] public string ThumbColor { get; set; } = "#7aa2f7";
  [Parameter] public int ContentWidth { get; set; }
  [Parameter] public int ContentHeight { get; set; }

  protected override ScrollBoxRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(ScrollBoxRenderable renderable)
  {
    renderable.ScrollX = ScrollX;
    renderable.ScrollY = ScrollY;
    renderable.StickyScroll = StickyScroll;
    renderable.ShowVerticalScrollbar = ShowVerticalScrollbar;
    renderable.ShowHorizontalScrollbar = ShowHorizontalScrollbar;
    renderable.Border = Border;
    renderable.BorderColor = BorderColor;
    renderable.BackgroundColor = BackgroundColor;
    renderable.TrackColor = TrackColor;
    renderable.ThumbColor = ThumbColor;
    renderable.ContentWidth = ContentWidth;
    renderable.ContentHeight = ContentHeight;
  }
}
