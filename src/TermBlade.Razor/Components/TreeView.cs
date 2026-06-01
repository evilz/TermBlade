using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class TreeView : RenderableComponentBase<TreeViewRenderable>
{
  [Parameter] public List<TreeNode> Nodes { get; set; } = [];
  [Parameter] public bool AllowLetterBasedNavigation { get; set; } = true;
  [Parameter] public bool CheckboxMode { get; set; } = false;
  [Parameter] public string? Fg { get; set; }
  [Parameter] public string? SelectedBg { get; set; } = "#0055aa";
  [Parameter] public string? Filter { get; set; }
  [Parameter] public EventCallback<TreeNode> OnSelectionChanged { get; set; }
  [Parameter] public EventCallback<TreeNode> OnNodeToggled { get; set; }
  [Parameter] public EventCallback<TreeNode> OnNodeChecked { get; set; }
  [Parameter] public EventCallback<TreeNode> OnNodeActivated { get; set; }

  protected override TreeViewRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void OnRenderableCreated(TreeViewRenderable renderable)
  {
    renderable.On("selectionChanged", data =>
    {
      if (data is not TreeNode node) return;
      DispatchEvent(async () =>
      {
        if (OnSelectionChanged.HasDelegate)
          await OnSelectionChanged.InvokeAsync(node).ConfigureAwait(false);
      });
    });

    renderable.On("nodeToggled", data =>
    {
      if (data is not TreeNode node) return;
      DispatchEvent(async () =>
      {
        if (OnNodeToggled.HasDelegate)
          await OnNodeToggled.InvokeAsync(node).ConfigureAwait(false);
      });
    });

    renderable.On("nodeChecked", data =>
    {
      if (data is not TreeNode node) return;
      DispatchEvent(async () =>
      {
        if (OnNodeChecked.HasDelegate)
          await OnNodeChecked.InvokeAsync(node).ConfigureAwait(false);
      });
    });

    renderable.On("nodeActivated", data =>
    {
      if (data is not TreeNode node) return;
      DispatchEvent(async () =>
      {
        if (OnNodeActivated.HasDelegate)
          await OnNodeActivated.InvokeAsync(node).ConfigureAwait(false);
      });
    });
  }

  protected override void ApplyParameters(TreeViewRenderable renderable)
  {
    renderable.Nodes = Nodes;
    renderable.AllowLetterBasedNavigation = AllowLetterBasedNavigation;
    renderable.CheckboxMode = CheckboxMode;
    renderable.Fg = Fg;
    renderable.SelectedBg = SelectedBg;
    renderable.Filter = Filter;
    renderable.Invalidate();
  }
}
