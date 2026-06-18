namespace TermBlade.Core.Layout;

/// <summary>
/// Defines flex direction values.
/// </summary>
public enum FlexDirection { Row, Column, RowReverse, ColumnReverse }
/// <summary>
/// Defines align items values.
/// </summary>
public enum AlignItems { FlexStart, FlexEnd, Center, Stretch, Baseline }
/// <summary>
/// Defines justify content values.
/// </summary>
public enum JustifyContent { FlexStart, FlexEnd, Center, SpaceBetween, SpaceAround, SpaceEvenly }

/// <summary>
/// Represents flex node.
/// </summary>
public class FlexNode
{
  /// <summary>
  /// Gets or sets the width.
  /// </summary>
  public LayoutDimension Width { get; set; } = LayoutDimension.Auto;
  /// <summary>
  /// Gets or sets the height.
  /// </summary>
  public LayoutDimension Height { get; set; } = LayoutDimension.Auto;
  /// <summary>
  /// Gets or sets the min width.
  /// </summary>
  public LayoutDimension MinWidth { get; set; } = LayoutDimension.Fixed(0);
  /// <summary>
  /// Gets or sets the max width.
  /// </summary>
  public LayoutDimension MaxWidth { get; set; } = LayoutDimension.Auto;
  /// <summary>
  /// Gets or sets the min height.
  /// </summary>
  public LayoutDimension MinHeight { get; set; } = LayoutDimension.Fixed(0);
  /// <summary>
  /// Gets or sets the max height.
  /// </summary>
  public LayoutDimension MaxHeight { get; set; } = LayoutDimension.Auto;
  /// <summary>
  /// Gets or sets the flex direction.
  /// </summary>
  public FlexDirection FlexDirection { get; set; } = FlexDirection.Row;
  /// <summary>
  /// Gets or sets the align items.
  /// </summary>
  public AlignItems AlignItems { get; set; } = AlignItems.FlexStart;
  /// <summary>
  /// Gets or sets the justify content.
  /// </summary>
  public JustifyContent JustifyContent { get; set; } = JustifyContent.FlexStart;
  /// <summary>
  /// Gets or sets the flex grow.
  /// </summary>
  public float FlexGrow { get; set; } = 0f;
  /// <summary>
  /// Gets or sets the flex shrink.
  /// </summary>
  public float FlexShrink { get; set; } = 1f;
  /// <summary>
  /// Gets or sets the flex basis.
  /// </summary>
  public LayoutDimension FlexBasis { get; set; } = LayoutDimension.Auto;
  /// <summary>
  /// Gets or sets the position.
  /// </summary>
  public string Position { get; set; } = "relative";
  /// <summary>
  /// Gets or sets the top.
  /// </summary>
  public int? Top { get; set; }
  /// <summary>
  /// Gets or sets the left.
  /// </summary>
  public int? Left { get; set; }
  /// <summary>
  /// Gets or sets the right.
  /// </summary>
  public int? Right { get; set; }
  /// <summary>
  /// Gets or sets the bottom.
  /// </summary>
  public int? Bottom { get; set; }
  /// <summary>
  /// Gets or sets the padding top.
  /// </summary>
  public int PaddingTop { get; set; }
  /// <summary>
  /// Gets or sets the padding right.
  /// </summary>
  public int PaddingRight { get; set; }
  /// <summary>
  /// Gets or sets the padding bottom.
  /// </summary>
  public int PaddingBottom { get; set; }
  /// <summary>
  /// Gets or sets the padding left.
  /// </summary>
  public int PaddingLeft { get; set; }
  /// <summary>
  /// Gets or sets the margin top.
  /// </summary>
  public int MarginTop { get; set; }
  /// <summary>
  /// Gets or sets the margin right.
  /// </summary>
  public int MarginRight { get; set; }
  /// <summary>
  /// Gets or sets the margin bottom.
  /// </summary>
  public int MarginBottom { get; set; }
  /// <summary>
  /// Gets or sets the margin left.
  /// </summary>
  public int MarginLeft { get; set; }
  /// <summary>
  /// Gets or sets the overflow.
  /// </summary>
  public string Overflow { get; set; } = "visible";

  /// <summary>
  /// Gets or sets the computed x.
  /// </summary>
  public int ComputedX { get; internal set; }
  /// <summary>
  /// Gets or sets the computed y.
  /// </summary>
  public int ComputedY { get; internal set; }
  /// <summary>
  /// Gets or sets the computed width.
  /// </summary>
  public int ComputedWidth { get; internal set; }
  /// <summary>
  /// Gets or sets the computed height.
  /// </summary>
  public int ComputedHeight { get; internal set; }

  /// <summary>
  /// Gets the children.
  /// </summary>
  public List<FlexNode> Children { get; } = new();

  /// <summary>
  /// Gets the add child.
  /// </summary>
  public void AddChild(FlexNode child) => Children.Add(child);
  /// <summary>
  /// Gets the remove child.
  /// </summary>
  public void RemoveChild(FlexNode child) => Children.Remove(child);

  /// <summary>
  /// Set padding.
  /// </summary>
  /// <param name="all">The all value.</param>
  public void SetPadding(int all) { PaddingTop = PaddingRight = PaddingBottom = PaddingLeft = all; }
  /// <summary>
  /// Set margin.
  /// </summary>
  /// <param name="all">The all value.</param>
  public void SetMargin(int all) { MarginTop = MarginRight = MarginBottom = MarginLeft = all; }
}
