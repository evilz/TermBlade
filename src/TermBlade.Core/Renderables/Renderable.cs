using TermBlade.Core.Events;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents renderable.
/// </summary>
public abstract class Renderable : EventEmitter
{
  private static int _idSeq;

  /// <summary>
  /// Gets or sets the id.
  /// </summary>
  public string Id { get; set; }
  /// <summary>
  /// Gets the layout node.
  /// </summary>
  public FlexNode LayoutNode { get; } = new();

  /// <summary>
  /// Flex direction.
  /// </summary>
  public virtual FlexDirection FlexDirection { get => LayoutNode.FlexDirection; set => LayoutNode.FlexDirection = value; }
  /// <summary>
  /// Align items.
  /// </summary>
  public virtual AlignItems AlignItems { get => LayoutNode.AlignItems; set => LayoutNode.AlignItems = value; }
  /// <summary>
  /// Justify content.
  /// </summary>
  public virtual JustifyContent JustifyContent { get => LayoutNode.JustifyContent; set => LayoutNode.JustifyContent = value; }
  /// <summary>
  /// Flex grow.
  /// </summary>
  public virtual float FlexGrow { get => LayoutNode.FlexGrow; set => LayoutNode.FlexGrow = value; }
  /// <summary>
  /// Flex shrink.
  /// </summary>
  public virtual float FlexShrink { get => LayoutNode.FlexShrink; set => LayoutNode.FlexShrink = value; }
  /// <summary>
  /// Position.
  /// </summary>
  public virtual string Position { get => LayoutNode.Position; set => LayoutNode.Position = value; }
  /// <summary>
  /// Top.
  /// </summary>
  public virtual int? Top { get => LayoutNode.Top; set => LayoutNode.Top = value; }
  /// <summary>
  /// Left.
  /// </summary>
  public virtual int? Left { get => LayoutNode.Left; set => LayoutNode.Left = value; }
  /// <summary>
  /// Right.
  /// </summary>
  public virtual int? Right { get => LayoutNode.Right; set => LayoutNode.Right = value; }
  /// <summary>
  /// Bottom.
  /// </summary>
  public virtual int? Bottom { get => LayoutNode.Bottom; set => LayoutNode.Bottom = value; }
  /// <summary>
  /// Padding top.
  /// </summary>
  public virtual int PaddingTop { get => LayoutNode.PaddingTop; set => LayoutNode.PaddingTop = value; }
  /// <summary>
  /// Padding right.
  /// </summary>
  public virtual int PaddingRight { get => LayoutNode.PaddingRight; set => LayoutNode.PaddingRight = value; }
  /// <summary>
  /// Padding bottom.
  /// </summary>
  public virtual int PaddingBottom { get => LayoutNode.PaddingBottom; set => LayoutNode.PaddingBottom = value; }
  /// <summary>
  /// Padding left.
  /// </summary>
  public virtual int PaddingLeft { get => LayoutNode.PaddingLeft; set => LayoutNode.PaddingLeft = value; }
  /// <summary>
  /// Margin top.
  /// </summary>
  public virtual int MarginTop { get => LayoutNode.MarginTop; set => LayoutNode.MarginTop = value; }
  /// <summary>
  /// Margin right.
  /// </summary>
  public virtual int MarginRight { get => LayoutNode.MarginRight; set => LayoutNode.MarginRight = value; }
  /// <summary>
  /// Margin bottom.
  /// </summary>
  public virtual int MarginBottom { get => LayoutNode.MarginBottom; set => LayoutNode.MarginBottom = value; }
  /// <summary>
  /// Margin left.
  /// </summary>
  public virtual int MarginLeft { get => LayoutNode.MarginLeft; set => LayoutNode.MarginLeft = value; }

  /// <summary>
  /// Gets the set width.
  /// </summary>
  public virtual void SetWidth(object? value) => LayoutNode.Width = LayoutDimension.Parse(value);
  /// <summary>
  /// Gets the set height.
  /// </summary>
  public virtual void SetHeight(object? value) => LayoutNode.Height = LayoutDimension.Parse(value);
  protected void SetInitialWidth(object? value) => LayoutNode.Width = LayoutDimension.Parse(value);
  protected void SetInitialHeight(object? value) => LayoutNode.Height = LayoutDimension.Parse(value);

  /// <summary>
  /// Gets the x.
  /// </summary>
  public int X => LayoutNode.ComputedX;
  /// <summary>
  /// Gets the y.
  /// </summary>
  public int Y => LayoutNode.ComputedY;
  /// <summary>
  /// Gets the computed width.
  /// </summary>
  public int ComputedWidth => LayoutNode.ComputedWidth;
  /// <summary>
  /// Gets the computed height.
  /// </summary>
  public int ComputedHeight => LayoutNode.ComputedHeight;

  /// <summary>
  /// Gets or sets the screen x.
  /// </summary>
  public int ScreenX { get; internal set; }
  /// <summary>
  /// Gets or sets the screen y.
  /// </summary>
  public int ScreenY { get; internal set; }

  /// <summary>
  /// Gets or sets the zindex.
  /// </summary>
  public int ZIndex { get; set; } = 0;
  /// <summary>
  /// Gets or sets the visible.
  /// </summary>
  public bool Visible { get; set; } = true;
  /// <summary>
  /// Gets or sets the opacity.
  /// </summary>
  public float Opacity { get; set; } = 1f;
  /// <summary>
  /// Gets or sets the focusable.
  /// </summary>
  public bool Focusable { get; set; } = false;
  /// <summary>
  /// Gets or sets the focused.
  /// </summary>
  public bool Focused { get; private set; } = false;

  /// <summary>
  /// Gets or sets the parent.
  /// </summary>
  public Renderable? Parent { get; private set; }
  protected CliRenderer? Renderer { get; private set; }

  private readonly List<Renderable> _children = new();

  protected Renderable(CliRenderer? renderer = null)
  {
    Id = $"renderable_{Interlocked.Increment(ref _idSeq)}";
    Renderer = renderer;
  }

  /// <summary>
  /// Add.
  /// </summary>
  /// <param name="child">The child value.</param>
  public void Add(Renderable child)
  {
    child.Parent = this;
    child.Renderer = Renderer;
    _children.Add(child);
    LayoutNode.AddChild(child.LayoutNode);
  }

  /// <summary>
  /// Remove.
  /// </summary>
  /// <param name="id">The id value.</param>
  public void Remove(string id)
  {
    var child = _children.FirstOrDefault(c => c.Id == id);
    child?.Parent = null;
    if (child == null) return;

    _children.Remove(child);
    LayoutNode.RemoveChild(child.LayoutNode);
  }

  /// <summary>
  /// Gets the get children.
  /// </summary>
  public List<Renderable> GetChildren() => new(_children);

  /// <summary>
  /// Focus.
  /// </summary>
  public void Focus()
  {
    if (Focused) return;
    Renderer?.OnRenderableFocused(this);
    Focused = true;
    Emit("focused");
  }

  /// <summary>
  /// Blur.
  /// </summary>
  public void Blur()
  {
    if (!Focused) return;
    Renderer?.OnRenderableBlurred(this);
    Focused = false;
    Emit("blurred");
  }

  /// <summary>
  /// Handle key.
  /// </summary>
  /// <param name="key">The key value.</param>
  public virtual void HandleKey(KeyEvent key) { }
  /// <summary>
  /// Handle mouse.
  /// </summary>
  /// <param name="mouse">The mouse value.</param>
  public virtual void HandleMouse(MouseEvent mouse) { }
  /// <summary>
  /// Gets the request render.
  /// </summary>
  public void RequestRender() => Renderer?.RequestRender();

  /// <summary>
  /// Destroy.
  /// </summary>
  public virtual void Destroy()
  {
    Emit("destroyed");
    foreach (var child in _children) child.Destroy();
    RemoveAllListeners();
  }

  /// <summary>
  /// Render.
  /// </summary>
  /// <param name="buffer">The buffer value.</param>
  /// <param name="deltaTime">The deltaTime value.</param>
  public virtual void Render(RenderBuffer buffer, double deltaTime)
  {
    if (!Visible) return;

    ScreenX = (Parent?.ScreenX ?? 0) + X;
    ScreenY = (Parent?.ScreenY ?? 0) + Y;

    RenderSelf(buffer, deltaTime);

    var sorted = _children.Where(c => c.Visible).OrderBy(c => c.ZIndex).ToList();
    foreach (var child in sorted)
      child.Render(buffer, deltaTime);
  }

  protected abstract void RenderSelf(RenderBuffer buffer, double deltaTime);
}
