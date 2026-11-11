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
  private volatile bool _destroyed;

  protected Renderable(CliRenderer? renderer = null)
  {
    Id = $"renderable_{Interlocked.Increment(ref _idSeq)}";
    Renderer = renderer;
  }

  /// <summary>
  /// Adds or reparents a child renderable.
  /// </summary>
  /// <param name="child">The child value.</param>
  /// <exception cref="ArgumentNullException">The child is null.</exception>
  /// <exception cref="ArgumentException">Adding the child would create a cycle.</exception>
  /// <exception cref="ObjectDisposedException">This renderable or the child was destroyed.</exception>
  public void Add(Renderable child)
  {
    ArgumentNullException.ThrowIfNull(child);
    if (_destroyed)
      throw new ObjectDisposedException(GetType().Name);
    if (child._destroyed)
      throw new ObjectDisposedException(child.GetType().Name);
    if (ReferenceEquals(child.Parent, this))
      return;

    for (Renderable? ancestor = this; ancestor != null; ancestor = ancestor.Parent)
    {
      if (ReferenceEquals(ancestor, child))
        throw new ArgumentException("Adding this renderable would create a cycle.", nameof(child));
    }

    if (child.Parent is { } previousParent)
      previousParent.Remove(child);
    else if (!ReferenceEquals(child.Renderer, Renderer))
      child.DetachRenderer();

    if (child._destroyed)
      throw new ObjectDisposedException(child.GetType().Name);

    child.Parent = this;
    child.SetRenderer(Renderer);
    _children.Add(child);
    LayoutNode.AddChild(child.LayoutNode);
  }

  /// <summary>
  /// Detaches a child renderable without destroying it.
  /// </summary>
  /// <param name="id">The id value.</param>
  public void Remove(string id)
  {
    var child = _children.FirstOrDefault(c => c.Id == id);
    if (child != null)
      Remove(child);
  }

  private void Remove(Renderable child)
  {
    if (!_children.Remove(child)) return;

    LayoutNode.RemoveChild(child.LayoutNode);
    if (ReferenceEquals(child.Parent, this))
      child.Parent = null;
    child.DetachRenderer();
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
    var renderer = Renderer;
    if (_destroyed || Focused || renderer == null) return;
    Focused = true;
    if (!renderer.OnRenderableFocused(this))
    {
      Focused = false;
      return;
    }
    Emit("focused");
  }

  /// <summary>
  /// Blur.
  /// </summary>
  public void Blur()
    => Blur(Renderer);

  private void Blur(CliRenderer? renderer)
  {
    if (!Focused) return;
    Focused = false;
    renderer?.OnRenderableBlurred(this);
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
  public void RequestRender()
  {
    if (!_destroyed)
      Renderer?.RequestRender();
  }

  /// <summary>
  /// Destroys this renderable and its children, detaches it from its parent and renderer,
  /// and releases owned resources. Repeated calls have no effect.
  /// </summary>
  public virtual void Destroy()
  {
    if (_destroyed) return;
    _destroyed = true;

    Exception? failure = null;
    try
    {
      if (Parent is { } parent)
        parent.Remove(this);
      else
        DetachRenderer();
    }
    catch (Exception exception)
    {
      failure = exception;
    }
    try
    {
      Emit("destroyed");
    }
    catch (Exception exception)
    {
      failure ??= exception;
    }

    foreach (var child in _children.ToArray())
    {
      try
      {
        child.Destroy();
      }
      catch (Exception exception)
      {
        failure ??= exception;
      }
    }

    try
    {
      DisposeResources();
    }
    catch (Exception exception)
    {
      failure ??= exception;
    }
    finally
    {
      RemoveAllListeners();
    }

    if (failure != null)
      System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
  }

  /// <summary>Releases resources owned by this renderable when it is destroyed.</summary>
  protected virtual void DisposeResources() { }

  /// <summary>
  /// Render.
  /// </summary>
  /// <param name="buffer">The buffer value.</param>
  /// <param name="deltaTime">The deltaTime value.</param>
  public virtual void Render(RenderBuffer buffer, double deltaTime)
  {
    if (_destroyed || !Visible) return;

    ScreenX = (Parent?.ScreenX ?? 0) + X;
    ScreenY = (Parent?.ScreenY ?? 0) + Y;

    RenderSelf(buffer, deltaTime);

    var sorted = _children.Where(c => c.Visible).OrderBy(c => c.ZIndex).ToList();
    foreach (var child in sorted)
      child.Render(buffer, deltaTime);
  }

  private bool Contains(Renderable renderable)
  {
    for (Renderable? current = renderable; current != null; current = current.Parent)
    {
      if (ReferenceEquals(current, this))
        return true;
    }

    return false;
  }

  private void DetachRenderer()
  {
    var owner = Renderer;
    var focused = owner?.CurrentFocus;
    var shouldBlur = focused != null && Contains(focused);
    SetRenderer(null);
    if (shouldBlur)
      focused!.Blur(owner);
  }

  private void SetRenderer(CliRenderer? renderer)
  {
    Renderer = renderer;
    foreach (var child in _children)
      child.SetRenderer(renderer);
  }

  protected abstract void RenderSelf(RenderBuffer buffer, double deltaTime);
}
