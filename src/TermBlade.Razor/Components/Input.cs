using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class Input : RenderableComponentBase<InputRenderable>
{
  [Parameter] public string? Value { get; set; }
  [Parameter] public EventCallback<string> ValueChanged { get; set; }
  [Parameter] public EventCallback<string> OnInput { get; set; }
  [Parameter] public EventCallback<string> OnChange { get; set; }
  [Parameter] public EventCallback<string> OnEnter { get; set; }
  [Parameter] public string? Placeholder { get; set; }
  [Parameter] public string? PlaceholderColor { get; set; }
  [Parameter] public string? CursorColor { get; set; }
  [Parameter] public string? Fg { get; set; }
  [Parameter] public string? Bg { get; set; }
  [Parameter] public int? MaxLength { get; set; }

  protected override InputRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(InputRenderable renderable)
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

    renderable.On("change", data =>
    {
      var value = data?.ToString() ?? string.Empty;
      DispatchEvent(async () =>
      {
        if (ValueChanged.HasDelegate)
          await ValueChanged.InvokeAsync(value).ConfigureAwait(false);
        if (OnChange.HasDelegate)
          await OnChange.InvokeAsync(value).ConfigureAwait(false);
      });
    });

    renderable.On("enter", data =>
    {
      var value = data?.ToString() ?? string.Empty;
      DispatchEvent(async () =>
      {
        if (OnEnter.HasDelegate)
          await OnEnter.InvokeAsync(value).ConfigureAwait(false);
      });
    });
  }

  protected override void ApplyParameters(InputRenderable renderable)
  {
    var value = Value ?? string.Empty;
    if (renderable.Value != value)
      renderable.Value = value;
    renderable.Placeholder = Placeholder;
    renderable.PlaceholderColor = PlaceholderColor;
    renderable.CursorColor = CursorColor;
    renderable.Fg = Fg;
    renderable.Bg = Bg;
    renderable.MaxLength = MaxLength;
  }
}
