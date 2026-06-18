using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Represents tab select.
/// </summary>
public sealed class TabSelect : RenderableComponentBase<TabSelectRenderable>
{
  [Parameter] public List<string> Tabs { get; set; } = [];
  [Parameter] public int SelectedIndex { get; set; }
  [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }
  [Parameter] public EventCallback<int> OnTabChanged { get; set; }
  [Parameter] public string? ActiveFg { get; set; }
  [Parameter] public string? ActiveBg { get; set; }
  [Parameter] public string? InactiveFg { get; set; }
  [Parameter] public string? InactiveBg { get; set; }

  protected override TabSelectRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(TabSelectRenderable renderable)
  {
    renderable.On("tabChanged", data =>
    {
      var value = Convert.ToInt32(data ?? 0);
      DispatchEvent(async () =>
      {
        if (SelectedIndexChanged.HasDelegate)
          await SelectedIndexChanged.InvokeAsync(value).ConfigureAwait(false);
        if (OnTabChanged.HasDelegate)
          await OnTabChanged.InvokeAsync(value).ConfigureAwait(false);
      });
    });
  }

  protected override void ApplyParameters(TabSelectRenderable renderable)
  {
    renderable.Tabs = Tabs;
    renderable.SelectedIndex = SelectedIndex;
    renderable.ActiveFg = ActiveFg;
    renderable.ActiveBg = ActiveBg;
    renderable.InactiveFg = InactiveFg;
    renderable.InactiveBg = InactiveBg;
  }
}
