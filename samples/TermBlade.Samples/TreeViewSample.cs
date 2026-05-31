using TermBlade.Core.Input;
using TermBlade.Core.Rendering;
using TermBlade.Core.Renderables;

namespace TermBlade.Samples;

internal static class TreeViewSample
{
  public static void Run()
  {
    var config = new CliRendererConfig { ExitOnCtrlC = true, TargetFps = 30 };
    var renderer = new CliRenderer(config);
    var root = renderer.Root;

    var box = new BoxRenderable(renderer, new BoxOptions
    {
      Border = true,
      BorderStyle = "rounded",
      BorderColor = "#7aa2f7",
      Title = " TreeView Component ",
      FlexDirection = TermBlade.Core.Layout.FlexDirection.Column,
      FlexGrow = 1
    });
    box.SetWidth("100%");
    box.SetHeight("100%");
    root.Add(box);

    var hint = new TextRenderable(renderer, new TextOptions
    {
      Content = "↑↓ navigate  ←→ collapse/expand  Space toggle checkbox  Enter activate  q quit",
      Fg = "#888888"
    });
    box.Add(hint);

    var tree = new TreeViewRenderable(renderer)
    {
      CheckboxMode = true,
      Nodes =
      [
        new TreeNode
        {
          Label = "src",
          IsExpanded = true,
          Children =
          [
            new TreeNode
            {
              Label = "TermBlade.Core",
              IsExpanded = true,
              Children =
              [
                new TreeNode { Label = "Renderables" },
                new TreeNode { Label = "Rendering" },
                new TreeNode { Label = "Layout" },
                new TreeNode { Label = "Input" }
              ]
            },
            new TreeNode
            {
              Label = "TermBlade.Razor",
              IsExpanded = false,
              Children =
              [
                new TreeNode { Label = "Components" },
                new TreeNode { Label = "Hosting" }
              ]
            }
          ]
        },
        new TreeNode
        {
          Label = "tests",
          IsExpanded = true,
          Children =
          [
            new TreeNode { Label = "TermBlade.Tests" }
          ]
        },
        new TreeNode
        {
          Label = "samples",
          IsExpanded = false,
          Children =
          [
            new TreeNode { Label = "TermBlade.Samples" },
            new TreeNode { Label = "TermBlade.Razor.Samples" },
            new TreeNode { Label = "TermBlade.Gallery" }
          ]
        }
      ]
    };
    tree.SetWidth("100%");
    tree.FlexGrow = 1;
    box.Add(tree);

    var status = new TextRenderable(renderer, new TextOptions
    {
      Content = "Navigate the tree",
      Fg = "#a9b1d6"
    });
    box.Add(status);

    tree.On("selectionChanged", (object? data) =>
    {
      if (data is TreeNode node)
      {
        status.Content = $"Selected: {node.Label}";
        renderer.RequestRender();
      }
    });

    tree.On("nodeToggled", (object? data) =>
    {
      if (data is TreeNode node)
      {
        status.Content = $"{node.Label} {(node.IsExpanded ? "expanded" : "collapsed")}";
        renderer.RequestRender();
      }
    });

    tree.On("nodeChecked", (object? data) =>
    {
      if (data is TreeNode node)
      {
        status.Content = $"Checked: {node.Label} = {node.CheckState}";
        renderer.RequestRender();
      }
    });

    tree.On("nodeActivated", (object? data) =>
    {
      if (data is TreeNode node)
      {
        status.Content = $"Activated: {node.Label}";
        renderer.RequestRender();
      }
    });

    renderer.KeyInput.On("keypress", (KeyEvent key) =>
    {
      if (key.Name == "q" || key.Name == "escape") renderer.Destroy();
    });

    tree.Focus();
    renderer.Start();
  }
}
