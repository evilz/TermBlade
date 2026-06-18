using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents select.
/// </summary>
public sealed class Select : RenderableComponentBase<SelectRenderable>
{
  [Parameter] public List<SelectOption> Options { get; set; } = [];
  [Parameter] public int SelectedIndex { get; set; }
  [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }
  [Parameter] public EventCallback<int> OnSelectionChanged { get; set; }
  [Parameter] public EventCallback<SelectOption> OnItemSelected { get; set; }
  [Parameter] public bool ShowDescription { get; set; } = true;
  [Parameter] public bool ShowScrollIndicator { get; set; } = true;
  [Parameter] public string? SelectedBg { get; set; } = "#0055aa";
  [Parameter] public string? Fg { get; set; }

  protected override SelectRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(SelectRenderable renderable)
  {
    renderable.On("selectionChanged", data =>
    {
      var value = Convert.ToInt32(data ?? 0);
      DispatchEvent(async () =>
      {
        if (SelectedIndexChanged.HasDelegate)
          await SelectedIndexChanged.InvokeAsync(value).ConfigureAwait(false);
        if (OnSelectionChanged.HasDelegate)
          await OnSelectionChanged.InvokeAsync(value).ConfigureAwait(false);
      });
    });

    renderable.On("itemSelected", data =>
    {
      if (data is not SelectOption option)
        return;

      DispatchEvent(async () =>
      {
        if (OnItemSelected.HasDelegate)
          await OnItemSelected.InvokeAsync(option).ConfigureAwait(false);
      });
    });
  }

  protected override void ApplyParameters(SelectRenderable renderable)
  {
    renderable.Options = Options;
    renderable.SelectedIndex = SelectedIndex;
    renderable.ShowDescription = ShowDescription;
    renderable.ShowScrollIndicator = ShowScrollIndicator;
    renderable.SelectedBg = SelectedBg;
    renderable.Fg = Fg;
  }
}
