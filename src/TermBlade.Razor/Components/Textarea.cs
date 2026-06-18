using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents textarea.
/// </summary>
public sealed class Textarea : RenderableComponentBase<TextareaRenderable>
{
  [Parameter] public string Value { get; set; } = string.Empty;
  [Parameter] public EventCallback<string> ValueChanged { get; set; }
  [Parameter] public EventCallback<string> OnInput { get; set; }
  [Parameter] public string? Placeholder { get; set; }
  [Parameter] public string WrapMode { get; set; } = "word";
  [Parameter] public string? Fg { get; set; }
  [Parameter] public string? Bg { get; set; }
  [Parameter] public bool ShowCursor { get; set; } = true;
  [Parameter] public string? CursorColor { get; set; }

  protected override TextareaRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(TextareaRenderable renderable)
  {
    renderable.On("input", data =>
    {
      var value = data?.ToString() ?? string.Empty;
      DispatchEvent(async () =>
      {
        if (ValueChanged.HasDelegate)
          await ValueChanged.InvokeAsync(value).ConfigureAwait(false);
        if (OnInput.HasDelegate)
          await OnInput.InvokeAsync(value).ConfigureAwait(false);
      });
    });
  }

  protected override void ApplyParameters(TextareaRenderable renderable)
  {
    renderable.Value = Value;
    renderable.Placeholder = Placeholder;
    renderable.WrapMode = WrapMode;
    renderable.Fg = Fg;
    renderable.Bg = Bg;
    renderable.ShowCursor = ShowCursor;
    renderable.CursorColor = CursorColor;
  }
}
