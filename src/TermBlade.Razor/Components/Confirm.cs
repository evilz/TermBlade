using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents confirm.
/// </summary>
public sealed class Confirm : RenderableComponentBase<ConfirmRenderable>
{
  [Parameter] public string Message { get; set; } = "Are you sure?";
  [Parameter] public bool Value { get; set; }
  [Parameter] public EventCallback<bool> ValueChanged { get; set; }
  [Parameter] public EventCallback<bool> OnConfirmed { get; set; }
  [Parameter] public string YesLabel { get; set; } = "Yes";
  [Parameter] public string NoLabel { get; set; } = "No";
  [Parameter] public string? Fg { get; set; }
  [Parameter] public string? Bg { get; set; }
  [Parameter] public string? ActiveBg { get; set; } = "#0055aa";

  protected override ConfirmRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(ConfirmRenderable renderable)
  {
    renderable.On("valueChanged", data =>
    {
      var value = data is bool b && b;
      DispatchEvent(async () =>
      {
        if (ValueChanged.HasDelegate)
          await ValueChanged.InvokeAsync(value).ConfigureAwait(false);
      });
    });

    renderable.On("confirmed", data =>
    {
      var value = data is bool b && b;
      DispatchEvent(async () =>
      {
        if (OnConfirmed.HasDelegate)
          await OnConfirmed.InvokeAsync(value).ConfigureAwait(false);
      });
    });
  }

  protected override void ApplyParameters(ConfirmRenderable renderable)
  {
    renderable.Message = Message;
    renderable.Value = Value;
    renderable.YesLabel = YesLabel;
    renderable.NoLabel = NoLabel;
    renderable.Fg = Fg;
    renderable.Bg = Bg;
    renderable.ActiveBg = ActiveBg;
  }
}
