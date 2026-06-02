using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class MultiSelect : RenderableComponentBase<MultiSelectRenderable>
{
  [Parameter] public List<SelectOption> Options { get; set; } = [];
  [Parameter] public HashSet<int> SelectedIndices { get; set; } = [];
  [Parameter] public EventCallback<HashSet<int>> SelectedIndicesChanged { get; set; }
  [Parameter] public EventCallback<HashSet<int>> OnSelectionChanged { get; set; }
  [Parameter] public EventCallback<HashSet<int>> OnConfirmed { get; set; }
  [Parameter] public bool ShowDescription { get; set; } = true;
  [Parameter] public bool ShowScrollIndicator { get; set; } = true;
  [Parameter] public string? CursorBg { get; set; } = "#333333";
  [Parameter] public string? SelectedBg { get; set; } = "#0055aa";
  [Parameter] public string? Fg { get; set; }

  protected override MultiSelectRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(MultiSelectRenderable renderable)
  {
    renderable.On("selectionChanged", data =>
    {
      if (data is not List<int> indices) return;
      DispatchEvent(async () =>
      {
        var indexSet = new HashSet<int>(indices);
        if (SelectedIndicesChanged.HasDelegate)
          await SelectedIndicesChanged.InvokeAsync(indexSet).ConfigureAwait(false);
        if (OnSelectionChanged.HasDelegate)
          await OnSelectionChanged.InvokeAsync(indexSet).ConfigureAwait(false);
      });
    });

    renderable.On("confirmed", data =>
    {
      if (data is not List<int> indices) return;
      DispatchEvent(async () =>
      {
        var indexSet = new HashSet<int>(indices);
        if (OnConfirmed.HasDelegate)
          await OnConfirmed.InvokeAsync(indexSet).ConfigureAwait(false);
      });
    });
  }

  protected override void ApplyParameters(MultiSelectRenderable renderable)
  {
    renderable.Options = Options;
    renderable.SelectedIndices = SelectedIndices;
    renderable.ShowDescription = ShowDescription;
    renderable.ShowScrollIndicator = ShowScrollIndicator;
    renderable.CursorBg = CursorBg;
    renderable.SelectedBg = SelectedBg;
    renderable.Fg = Fg;
  }
}
