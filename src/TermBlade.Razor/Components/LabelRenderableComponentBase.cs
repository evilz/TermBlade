using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Base class for text-like components that expose common text parameters.
/// </summary>
public abstract class LabelRenderableComponentBase<TRenderable> : RenderableComponentBase<TRenderable> where TRenderable : LabelRenderable
{
  [Parameter] public string Content { get; set; } = string.Empty;
  [Parameter] public string? Fg { get; set; }
  [Parameter] public string? Bg { get; set; }
  [Parameter] public string TextAlign { get; set; } = "left";
  [Parameter] public TextAttributes Attributes { get; set; } = TextAttributes.None;

  protected override void ApplyParameters(TRenderable renderable)
  {
    renderable.Content = Content;
    renderable.Fg = Fg;
    renderable.Bg = Bg;
    renderable.TextAlign = TextAlign;
    renderable.Attributes = Attributes;
  }
}
