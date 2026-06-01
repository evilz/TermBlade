using TermBlade.Core.Input;
using TermBlade.Core.Renderables;

namespace TermBlade.Tests;

public class TreeViewTests
{
  private static TreeViewRenderable CreateTree()
  {
    var tree = new TreeViewRenderable(null)
    {
      Nodes =
      [
        new TreeNode
        {
          Label = "Root1",
          IsExpanded = true,
          Children =
          [
            new TreeNode { Label = "Child1.1" },
            new TreeNode { Label = "Child1.2" }
          ]
        },
        new TreeNode
        {
          Label = "Root2",
          IsExpanded = false,
          Children =
          [
            new TreeNode { Label = "Child2.1" }
          ]
        },
        new TreeNode { Label = "Root3" }
      ]
    };
    tree.RebuildFlatList();
    return tree;
  }

  [Fact]
  public void RebuildFlatList_ExpandedNodes_IncludesChildren()
  {
    var tree = CreateTree();

    // Root1 (expanded), Child1.1, Child1.2, Root2 (collapsed), Root3
    Assert.Equal("Root1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.2", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Root2", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Root3", tree.SelectedNode!.Label);
  }

  [Fact]
  public void RebuildFlatList_CollapsedNodes_ExcludesChildren()
  {
    var tree = new TreeViewRenderable(null)
    {
      Nodes =
      [
        new TreeNode
        {
          Label = "Root1",
          IsExpanded = false,
          Children = [new TreeNode { Label = "Child1.1" }]
        },
        new TreeNode { Label = "Root2" }
      ]
    };
    tree.RebuildFlatList();

    // Root1 (collapsed) + Root2 only — children are hidden
    Assert.Equal("Root1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Root2", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Navigation_Down_MovesToNextVisibleNode()
  {
    var tree = CreateTree();
    Assert.Equal("Root1", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.2", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Root2", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Navigation_Up_MovesToPreviousVisibleNode()
  {
    var tree = CreateTree();
    tree.HandleKey(new KeyEvent { Name = "down" });
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.2", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "up" });
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "up" });
    Assert.Equal("Root1", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Navigation_Up_AtFirst_DoesNotWrap()
  {
    var tree = CreateTree();
    Assert.Equal("Root1", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "up" });
    Assert.Equal("Root1", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Navigation_Down_AtLast_DoesNotWrap()
  {
    var tree = CreateTree();
    for (int i = 0; i < 10; i++)
      tree.HandleKey(new KeyEvent { Name = "down" });

    Assert.Equal("Root3", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Right_ExpandsCollapsedNode()
  {
    var tree = CreateTree();
    // Navigate to Root2 (collapsed)
    tree.HandleKey(new KeyEvent { Name = "down" }); // Child1.1
    tree.HandleKey(new KeyEvent { Name = "down" }); // Child1.2
    tree.HandleKey(new KeyEvent { Name = "down" }); // Root2
    Assert.Equal("Root2", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "right" });
    Assert.True(tree.SelectedNode!.IsExpanded);
    // Now Child2.1 should be visible — pressing Down reaches it
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child2.1", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Right_OnExpandedNode_MovesToFirstChild()
  {
    var tree = CreateTree();
    Assert.Equal("Root1", tree.SelectedNode!.Label);
    Assert.True(tree.SelectedNode!.IsExpanded);

    tree.HandleKey(new KeyEvent { Name = "right" });
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Left_CollapsesExpandedNode()
  {
    var tree = CreateTree();
    Assert.Equal("Root1", tree.SelectedNode!.Label);
    Assert.True(tree.SelectedNode!.IsExpanded);

    tree.HandleKey(new KeyEvent { Name = "left" });
    Assert.False(tree.SelectedNode!.IsExpanded);
    // Children should no longer be reachable by Down
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Root2", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Left_OnChildNode_JumpsToParent()
  {
    var tree = CreateTree();
    tree.HandleKey(new KeyEvent { Name = "down" }); // Child1.1
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);

    tree.HandleKey(new KeyEvent { Name = "left" });
    Assert.Equal("Root1", tree.SelectedNode!.Label);
  }

  [Fact]
  public void Enter_TogglesExpandCollapse()
  {
    var tree = CreateTree();
    Assert.True(tree.SelectedNode!.IsExpanded);

    tree.HandleKey(new KeyEvent { Name = "return" });
    Assert.False(tree.SelectedNode!.IsExpanded);

    tree.HandleKey(new KeyEvent { Name = "return" });
    Assert.True(tree.SelectedNode!.IsExpanded);
  }

  [Fact]
  public void Enter_OnLeafNode_EmitsNodeActivated()
  {
    var tree = CreateTree();
    tree.HandleKey(new KeyEvent { Name = "down" }); // Child1.1 (leaf)
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);
    Assert.Empty(tree.SelectedNode!.Children);

    object? activated = null;
    tree.On("nodeActivated", data => activated = data);

    tree.HandleKey(new KeyEvent { Name = "return" });
    Assert.NotNull(activated);
    Assert.Equal("Child1.1", ((TreeNode)activated!).Label);
  }

  [Fact]
  public void Checkbox_Space_TogglesUncheckedToChecked()
  {
    var tree = CreateTree();
    tree.CheckboxMode = true;
    var node = tree.SelectedNode!;
    Assert.Equal(CheckState.Unchecked, node.CheckState);

    tree.HandleKey(new KeyEvent { Name = "space" });
    Assert.Equal(CheckState.Checked, node.CheckState);
  }

  [Fact]
  public void Checkbox_PrintableSpaceCharacter_TogglesUncheckedToChecked()
  {
    var tree = CreateTree();
    tree.CheckboxMode = true;
    var node = tree.SelectedNode!;
    Assert.Equal(CheckState.Unchecked, node.CheckState);

    tree.HandleKey(new KeyEvent { Name = " ", Key = " ", Char = ' ' });
    Assert.Equal(CheckState.Checked, node.CheckState);
  }

  [Fact]
  public void Checkbox_Space_TogglesCheckedToUnchecked()
  {
    var tree = CreateTree();
    tree.CheckboxMode = true;
    var node = tree.SelectedNode!;
    node.CheckState = CheckState.Checked;

    tree.HandleKey(new KeyEvent { Name = "space" });
    Assert.Equal(CheckState.Unchecked, node.CheckState);
  }

  [Fact]
  public void Checkbox_CheckingParent_SetsAllChildrenChecked()
  {
    var tree = CreateTree();
    tree.CheckboxMode = true;

    // Select Root1 and check it
    Assert.Equal("Root1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "space" });

    var root1 = tree.SelectedNode!;
    Assert.Equal(CheckState.Checked, root1.CheckState);
    foreach (var child in root1.Children)
      Assert.Equal(CheckState.Checked, child.CheckState);
  }

  [Fact]
  public void Checkbox_PartialChildCheck_MakesParentIndeterminate()
  {
    var tree = CreateTree();
    tree.CheckboxMode = true;

    // Navigate to Child1.1 and check it
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "space" });

    // Root1 should now be indeterminate (1 of 2 children checked)
    var root1 = tree.Nodes[0];
    Assert.Equal(CheckState.Indeterminate, root1.CheckState);
  }

  [Fact]
  public void Checkbox_AllChildrenChecked_MakesParentChecked()
  {
    var tree = CreateTree();
    tree.CheckboxMode = true;

    // Check Child1.1
    tree.HandleKey(new KeyEvent { Name = "down" });
    tree.HandleKey(new KeyEvent { Name = "space" });

    // Check Child1.2
    tree.HandleKey(new KeyEvent { Name = "down" });
    tree.HandleKey(new KeyEvent { Name = "space" });

    var root1 = tree.Nodes[0];
    Assert.Equal(CheckState.Checked, root1.CheckState);
  }

  [Fact]
  public void Filter_ShowsOnlyMatchingNodes()
  {
    var tree = new TreeViewRenderable(null)
    {
      Filter = "Child1",
      Nodes =
      [
        new TreeNode
        {
          Label = "Root1",
          IsExpanded = true,
          Children =
          [
            new TreeNode { Label = "Child1.1" },
            new TreeNode { Label = "Child1.2" }
          ]
        },
        new TreeNode
        {
          Label = "Root2",
          IsExpanded = true,
          Children =
          [
            new TreeNode { Label = "Child2.1" }
          ]
        }
      ]
    };
    tree.RebuildFlatList();

    // Root1 should be visible (has matching descendants), Root2 should not (no match)
    Assert.Equal("Root1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.2", tree.SelectedNode!.Label);

    // Next down should not go to Root2's child
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.2", tree.SelectedNode!.Label); // stays at last
  }

  [Fact]
  public void Filter_ShowsMatchingDescendantsEvenWhenAncestorCollapsed()
  {
    var tree = new TreeViewRenderable(null)
    {
      Filter = "Child1",
      Nodes =
      [
        new TreeNode
        {
          Label = "Root1",
          IsExpanded = false,
          Children =
          [
            new TreeNode { Label = "Child1.1" },
            new TreeNode { Label = "Child1.2" }
          ]
        }
      ]
    };
    tree.RebuildFlatList();

    Assert.Equal("Root1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.1", tree.SelectedNode!.Label);
    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal("Child1.2", tree.SelectedNode!.Label);
  }

  [Fact]
  public void LetterNavigation_JumpsToMatchingNode()
  {
    var tree = CreateTree();
    tree.AllowLetterBasedNavigation = true;

    // Pressing 'r' should jump to Root2 (since Root1 is current)
    tree.HandleKey(new KeyEvent { Name = "r", Char = 'r' });
    Assert.Equal("Root2", tree.SelectedNode!.Label);
  }

  [Fact]
  public void LetterNavigation_Disabled_DoesNotJump()
  {
    var tree = CreateTree();
    tree.AllowLetterBasedNavigation = false;

    tree.HandleKey(new KeyEvent { Name = "r", Char = 'r' });
    Assert.Equal("Root1", tree.SelectedNode!.Label);
  }

  [Fact]
  public void SelectionChanged_Event_FiresOnNavigation()
  {
    var tree = CreateTree();
    object? changedTo = null;
    tree.On("selectionChanged", data => changedTo = data);

    tree.HandleKey(new KeyEvent { Name = "down" });
    Assert.NotNull(changedTo);
    Assert.Equal("Child1.1", ((TreeNode)changedTo!).Label);
  }

  [Fact]
  public void NodeToggled_Event_FiresOnExpand()
  {
    var tree = CreateTree();
    // Navigate to Root2 (collapsed)
    tree.HandleKey(new KeyEvent { Name = "down" }); // Child1.1
    tree.HandleKey(new KeyEvent { Name = "down" }); // Child1.2
    tree.HandleKey(new KeyEvent { Name = "down" }); // Root2

    object? toggled = null;
    tree.On("nodeToggled", data => toggled = data);

    tree.HandleKey(new KeyEvent { Name = "right" });
    Assert.NotNull(toggled);
    Assert.Equal("Root2", ((TreeNode)toggled!).Label);
  }

  [Fact]
  public void MouseClick_SelectsClickedRow()
  {
    var tree = CreateTree();
    tree.HandleMouse(new MouseEvent { X = 0, Y = 3, Button = MouseButton.Left, Pressed = true });
    Assert.Equal("Root2", tree.SelectedNode!.Label);
  }

  [Fact]
  public void MouseClick_OnCheckbox_TogglesNode()
  {
    var tree = CreateTree();
    tree.CheckboxMode = true;
    Assert.Equal(CheckState.Unchecked, tree.SelectedNode!.CheckState);

    tree.HandleMouse(new MouseEvent { X = 5, Y = 0, Button = MouseButton.Left, Pressed = true });
    Assert.Equal(CheckState.Checked, tree.SelectedNode!.CheckState);
  }

  [Fact]
  public void MouseClick_OnExpandIndicator_TogglesNode()
  {
    var tree = CreateTree();
    Assert.True(tree.SelectedNode!.IsExpanded);

    tree.HandleMouse(new MouseEvent { X = 2, Y = 0, Button = MouseButton.Left, Pressed = true });
    Assert.False(tree.SelectedNode!.IsExpanded);

    tree.HandleMouse(new MouseEvent { X = 2, Y = 0, Button = MouseButton.Left, Pressed = true });
    Assert.True(tree.SelectedNode!.IsExpanded);
  }
}
