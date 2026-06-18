using System.Text;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Defines check state values.
/// </summary>
public enum CheckState
{
  /// <summary>
  /// The unchecked value.
  /// </summary>
  Unchecked,
  /// <summary>
  /// The checked value.
  /// </summary>
  Checked,
  /// <summary>
  /// The indeterminate value.
  /// </summary>
  Indeterminate
}

/// <summary>
/// Represents tree node.
/// </summary>
public class TreeNode
{
  /// <summary>
  /// Gets or sets the label.
  /// </summary>
  public string Label { get; set; } = "";
  /// <summary>
  /// Gets or sets the children.
  /// </summary>
  public List<TreeNode> Children { get; set; } = new();
  /// <summary>
  /// Gets or sets the is expanded.
  /// </summary>
  public bool IsExpanded { get; set; } = true;
  /// <summary>
  /// Gets or sets the check state.
  /// </summary>
  public CheckState CheckState { get; set; } = CheckState.Unchecked;
  /// <summary>
  /// Gets or sets the tag.
  /// </summary>
  public object? Tag { get; set; }
}

/// <summary>
/// Represents tree view renderable.
/// </summary>
public class TreeViewRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the nodes.
  /// </summary>
  public List<TreeNode> Nodes { get; set; } = new();
  /// <summary>
  /// Gets or sets the allow letter based navigation.
  /// </summary>
  public bool AllowLetterBasedNavigation { get; set; } = true;
  /// <summary>
  /// Gets or sets the checkbox mode.
  /// </summary>
  public bool CheckboxMode { get; set; } = false;
  /// <summary>
  /// Gets or sets the fg.
  /// </summary>
  public string? Fg { get; set; }
  /// <summary>
  /// Gets or sets the selected bg.
  /// </summary>
  public string? SelectedBg { get; set; } = "#0055aa";
  /// <summary>
  /// Gets or sets the filter.
  /// </summary>
  public string? Filter { get; set; }

  private readonly record struct FlatNode(TreeNode Node, int Depth, bool IsLast, bool[] ParentIsLast, bool IsExpandedInView);

  private readonly List<FlatNode> _flatNodes = new();
  private int _selectedIndex = 0;
  private int _scrollOffset = 0;
  private bool _dirty = true;

  private string _keystrokeBuffer = "";
  private DateTime _lastKeystroke = DateTime.MinValue;
  private static readonly TimeSpan KeystrokeTimeout = TimeSpan.FromMilliseconds(600);

  /// <summary>
  /// Member.
  /// </summary>
  public int SelectedIndex
  {
    get => _selectedIndex;
    set
    {
      _selectedIndex = Math.Clamp(value, 0, Math.Max(0, _flatNodes.Count - 1));
      EnsureVisible();
    }
  }

  /// <summary>
  /// Selected node.
  /// </summary>
  public TreeNode? SelectedNode =>
      _flatNodes.Count > 0 && _selectedIndex < _flatNodes.Count
          ? _flatNodes[_selectedIndex].Node
          : null;

  /// <summary>
  /// Tree view renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public TreeViewRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }

  /// <summary>
  /// Gets the invalidate.
  /// </summary>
  public void Invalidate() => _dirty = true;

  /// <summary>
  /// Rebuild flat list.
  /// </summary>
  public void RebuildFlatList()
  {
    _flatNodes.Clear();
    BuildFlatListRecursive(Nodes, 0, Array.Empty<bool>());
    _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _flatNodes.Count - 1));
    EnsureVisible();
    _dirty = false;
  }

  private void BuildFlatListRecursive(IReadOnlyList<TreeNode> nodes, int depth, bool[] parentIsLast)
  {
    var filter = string.IsNullOrEmpty(Filter) ? null : Filter;

    for (int i = 0; i < nodes.Count; i++)
    {
      var node = nodes[i];
      bool isLast = i == nodes.Count - 1;

      if (filter != null)
      {
        bool matchesSelf = node.Label.Contains(filter, StringComparison.OrdinalIgnoreCase);
        if (!matchesSelf && !HasMatchingDescendant(node, filter))
          continue;
      }

      bool isExpandedInView = node.Children.Count > 0 && (node.IsExpanded || filter != null);
      _flatNodes.Add(new FlatNode(node, depth, isLast, parentIsLast, isExpandedInView));

      if (isExpandedInView)
      {
        bool[] childParentIsLast = [.. parentIsLast, isLast];
        BuildFlatListRecursive(node.Children, depth + 1, childParentIsLast);
      }
    }
  }

  private static bool HasMatchingDescendant(TreeNode node, string filter)
  {
    foreach (var child in node.Children)
    {
      if (child.Label.Contains(filter, StringComparison.OrdinalIgnoreCase)) return true;
      if (HasMatchingDescendant(child, filter)) return true;
    }
    return false;
  }

  /// <summary>
  /// Handle key.
  /// </summary>
  /// <param name="key">The key value.</param>
  public override void HandleKey(KeyEvent key)
  {
    if (_dirty) RebuildFlatList();
    if (_flatNodes.Count == 0) return;

    switch (key.Name)
    {
      case "up":
        if (_selectedIndex > 0)
        {
          _selectedIndex--;
          EnsureVisible();
          Emit("selectionChanged", SelectedNode);
          RequestRender();
        }
        break;

      case "down":
        if (_selectedIndex < _flatNodes.Count - 1)
        {
          _selectedIndex++;
          EnsureVisible();
          Emit("selectionChanged", SelectedNode);
          RequestRender();
        }
        break;

      case "right":
        {
          var node = _flatNodes[_selectedIndex].Node;
          if (node.Children.Count > 0)
          {
            if (!node.IsExpanded)
            {
              node.IsExpanded = true;
              RebuildFlatList();
              Emit("nodeToggled", node);
              RequestRender();
            }
            else if (_selectedIndex < _flatNodes.Count - 1)
            {
              _selectedIndex++;
              EnsureVisible();
              Emit("selectionChanged", SelectedNode);
              RequestRender();
            }
          }
          break;
        }

      case "left":
        {
          var (node, depth, _, _, _) = _flatNodes[_selectedIndex];
          if (node.IsExpanded && node.Children.Count > 0)
          {
            node.IsExpanded = false;
            RebuildFlatList();
            Emit("nodeToggled", node);
            RequestRender();
          }
          else if (depth > 0)
          {
            for (int i = _selectedIndex - 1; i >= 0; i--)
            {
              if (_flatNodes[i].Depth < depth)
              {
                _selectedIndex = i;
                EnsureVisible();
                Emit("selectionChanged", SelectedNode);
                RequestRender();
                break;
              }
            }
          }
          break;
        }

      case "return":
        {
          var node = _flatNodes[_selectedIndex].Node;
          if (node.Children.Count > 0)
          {
            node.IsExpanded = !node.IsExpanded;
            RebuildFlatList();
            Emit("nodeToggled", node);
          }
          else
          {
            Emit("nodeActivated", node);
          }
          RequestRender();
          break;
        }

      case "space":
      case " ":
        if (CheckboxMode)
        {
          ToggleCheckbox(_flatNodes[_selectedIndex].Node);
          Emit("nodeChecked", SelectedNode);
          RequestRender();
        }
        break;

      default:
        if (AllowLetterBasedNavigation && key.Char.HasValue && !key.Ctrl && !key.Alt)
          HandleLetterNavigation(key.Char.Value);
        break;
    }
  }

  /// <summary>
  /// Handle mouse.
  /// </summary>
  /// <param name="mouse">The mouse value.</param>
  public override void HandleMouse(MouseEvent mouse)
  {
    if (mouse.Button != MouseButton.Left || !mouse.Pressed)
      return;

    if (_dirty) RebuildFlatList();
    if (_flatNodes.Count == 0)
      return;

    int column = mouse.X - ScreenX;
    if (column < 0 || (ComputedWidth > 0 && column >= ComputedWidth))
      return;

    int row = mouse.Y - ScreenY;
    if (row < 0 || (ComputedHeight > 0 && row >= ComputedHeight))
      return;

    int idx = row + _scrollOffset;
    if (idx < 0 || idx >= _flatNodes.Count)
      return;

    bool selectionChanged = _selectedIndex != idx;
    _selectedIndex = idx;
    EnsureVisible();

    var flatNode = _flatNodes[idx];
    bool toggledNode = false;

    int expandColumn = flatNode.Depth * 3 + 2;
    if (flatNode.Node.Children.Count > 0 && column == expandColumn)
    {
      flatNode.Node.IsExpanded = !flatNode.Node.IsExpanded;
      RebuildFlatList();
      Emit("nodeToggled", flatNode.Node);
      toggledNode = true;
    }

    if (CheckboxMode)
    {
      int checkboxColumnStart = flatNode.Depth * 3 + 4;
      if (column >= checkboxColumnStart && column <= checkboxColumnStart + 2)
      {
        ToggleCheckbox(flatNode.Node);
        Emit("nodeChecked", flatNode.Node);
      }
    }

    if (selectionChanged)
      Emit("selectionChanged", SelectedNode);

    if (selectionChanged || toggledNode || CheckboxMode)
      RequestRender();
  }

  private void HandleLetterNavigation(char ch)
  {
    var now = DateTime.UtcNow;
    if (now - _lastKeystroke > KeystrokeTimeout)
      _keystrokeBuffer = "";
    _keystrokeBuffer += char.ToLowerInvariant(ch);
    _lastKeystroke = now;

    int startIdx = (_selectedIndex + 1) % _flatNodes.Count;
    for (int i = 0; i < _flatNodes.Count; i++)
    {
      int idx = (startIdx + i) % _flatNodes.Count;
      if (_flatNodes[idx].Node.Label.StartsWith(_keystrokeBuffer, StringComparison.OrdinalIgnoreCase))
      {
        _selectedIndex = idx;
        EnsureVisible();
        Emit("selectionChanged", SelectedNode);
        RequestRender();
        return;
      }
    }
  }

  private void ToggleCheckbox(TreeNode node)
  {
    var next = node.CheckState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;
    SetCheckStateRecursive(node, next);
    UpdateParentCheckStates();
  }

  private static void SetCheckStateRecursive(TreeNode node, CheckState state)
  {
    node.CheckState = state;
    foreach (var child in node.Children)
      SetCheckStateRecursive(child, state);
  }

  private void UpdateParentCheckStates()
  {
    foreach (var root in Nodes)
      ComputeCheckStateRecursive(root);
  }

  private static CheckState ComputeCheckStateRecursive(TreeNode node)
  {
    if (node.Children.Count == 0)
      return node.CheckState;

    int checkedCount = 0, uncheckedCount = 0;
    foreach (var child in node.Children)
    {
      var cs = ComputeCheckStateRecursive(child);
      if (cs == CheckState.Checked) checkedCount++;
      else if (cs == CheckState.Unchecked) uncheckedCount++;
    }

    node.CheckState = checkedCount == node.Children.Count
        ? CheckState.Checked
        : uncheckedCount == node.Children.Count
            ? CheckState.Unchecked
            : CheckState.Indeterminate;

    return node.CheckState;
  }

  private void EnsureVisible()
  {
    int h = ComputedHeight > 0 ? ComputedHeight : 1;
    if (_selectedIndex < _scrollOffset)
      _scrollOffset = _selectedIndex;
    if (_selectedIndex >= _scrollOffset + h)
      _scrollOffset = _selectedIndex - h + 1;
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    if (_dirty) RebuildFlatList();

    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(220, 220, 220);
    var bg = Rgba.FromInts(0, 0, 0);
    var selBg = SelectedBg != null ? Rgba.FromCss(SelectedBg) : Rgba.FromInts(0, 85, 170);

    for (int row = 0; row < h; row++)
    {
      int idx = row + _scrollOffset;
      if (idx >= _flatNodes.Count) break;

      var (node, depth, isLast, parentIsLast, isExpandedInView) = _flatNodes[idx];
      bool isSelected = idx == _selectedIndex;
      var rowBg = isSelected ? selBg : bg;

      buffer.FillRect(x, y + row, w, 1, rowBg);

      var sb = new StringBuilder();

      // Ancestor continuation lines
      for (int d = 0; d < depth; d++)
        sb.Append(parentIsLast[d] ? "   " : "│  ");

      // Branch connector
      sb.Append(isLast ? "└─" : "├─");

      // Expand/collapse indicator
      if (node.Children.Count > 0)
        sb.Append(isExpandedInView ? '▼' : '▶');
      else
        sb.Append(' ');

      // Checkbox glyph
      if (CheckboxMode)
      {
        sb.Append(' ');
        sb.Append(node.CheckState switch
        {
          CheckState.Checked => "[x]",
          CheckState.Indeterminate => "[-]",
          _ => "[ ]"
        });
      }

      sb.Append(' ');
      sb.Append(node.Label);

      var text = sb.ToString();
      // Truncate to avoid overflowing the component width
      if (text.Length > w)
        text = text[..w];

      buffer.DrawText(x, y + row, text, fg, rowBg);
    }

    // Scroll indicator
    if (_flatNodes.Count > h)
    {
      int scrollX = x + w - 1;
      var trackBg = Rgba.FromInts(30, 30, 30);
      var trackFg = Rgba.FromInts(70, 70, 70);
      var thumbFg = Rgba.FromInts(150, 150, 150);
      for (int row = 0; row < h; row++)
        buffer.SetCell(scrollX, y + row, '│', trackFg, trackBg);

      float ratio = (float)_scrollOffset / Math.Max(1, _flatNodes.Count - h);
      int thumbY = (int)(ratio * (h - 1));
      buffer.SetCell(scrollX, y + thumbY, '█', thumbFg, trackBg);
    }
  }
}
