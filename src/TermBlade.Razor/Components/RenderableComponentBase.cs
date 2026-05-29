using Microsoft.AspNetCore.Components;
using TermBlade.Core.Layout;
using TermBlade.Core.Renderables;
using TermBlade.Razor.Hosting;

namespace TermBlade.Razor.Components;

public abstract class RenderableComponentBase<TRenderable> : ComponentBase, IDisposable where TRenderable : Renderable
{
  private bool _initialized;
  private CommonParameterState _initialState;

  [Inject] protected TermBladeAppContext App { get; set; } = null!;
  [CascadingParameter] protected IRenderableParent? Parent { get; set; }

  [Parameter] public string? Id { get; set; }
  [Parameter] public string? Width { get; set; }
  [Parameter] public string? Height { get; set; }
  [Parameter] public FlexDirection? FlexDirection { get; set; }
  [Parameter] public AlignItems? AlignItems { get; set; }
  [Parameter] public JustifyContent? JustifyContent { get; set; }
  [Parameter] public float? FlexGrow { get; set; }
  [Parameter] public float? FlexShrink { get; set; }
  [Parameter] public string? LayoutPosition { get; set; }
  [Parameter] public int? Top { get; set; }
  [Parameter] public int? Left { get; set; }
  [Parameter] public int? Right { get; set; }
  [Parameter] public int? Bottom { get; set; }
  [Parameter] public int? PaddingTop { get; set; }
  [Parameter] public int? PaddingRight { get; set; }
  [Parameter] public int? PaddingBottom { get; set; }
  [Parameter] public int? PaddingLeft { get; set; }
  [Parameter] public int? MarginTop { get; set; }
  [Parameter] public int? MarginRight { get; set; }
  [Parameter] public int? MarginBottom { get; set; }
  [Parameter] public int? MarginLeft { get; set; }
  [Parameter] public int? ZIndex { get; set; }
  [Parameter] public bool? Visible { get; set; }
  [Parameter] public float? Opacity { get; set; }
  [Parameter] public bool? Focusable { get; set; }
  [Parameter] public bool AutoFocus { get; set; }

  public TRenderable Renderable { get; private set; } = null!;

  protected override void OnInitialized()
  {
    Renderable = CreateRenderable(App.Renderer);
    _initialState = CommonParameterState.Capture(Renderable);
    OnRenderableCreated(Renderable);

    if (Parent != null)
      Parent.AddChild(Renderable);
    else
      App.Renderer.Root.Add(Renderable);

    ApplyCommonParameters(Renderable);
    ApplyParameters(Renderable);
    _initialized = true;
    App.RequestRender();
  }

  protected override void OnParametersSet()
  {
    if (!_initialized)
      return;

    ApplyCommonParameters(Renderable);
    ApplyParameters(Renderable);
    App.RequestRender();
  }

  protected override void OnAfterRender(bool firstRender)
  {
    if (firstRender && AutoFocus)
    {
      Renderable.Focus();
      App.RequestRender();
    }
  }

  protected virtual void OnRenderableCreated(TRenderable renderable)
  {
  }

  protected abstract TRenderable CreateRenderable(TermBlade.Core.Rendering.CliRenderer renderer);

  protected abstract void ApplyParameters(TRenderable renderable);

  protected void InvokeRender() => App.RequestRender();

  protected void DispatchEvent(Func<Task> callback)
  {
    _ = InvokeAsync(async () =>
    {
      try
      {
        await callback().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        await DispatchExceptionAsync(ex).ConfigureAwait(false);
      }
    });
  }

  private void ApplyCommonParameters(TRenderable renderable)
  {
    _initialState.Apply(renderable);

    renderable.Id = Id ?? _initialState.Id;
    renderable.SetWidth(Width);
    renderable.SetHeight(Height);
    renderable.FlexDirection = FlexDirection ?? _initialState.FlexDirection;
    renderable.AlignItems = AlignItems ?? _initialState.AlignItems;
    renderable.JustifyContent = JustifyContent ?? _initialState.JustifyContent;
    renderable.FlexGrow = FlexGrow ?? _initialState.FlexGrow;
    renderable.FlexShrink = FlexShrink ?? _initialState.FlexShrink;
    renderable.Position = LayoutPosition ?? _initialState.Position;
    renderable.Top = Top ?? _initialState.Top;
    renderable.Left = Left ?? _initialState.Left;
    renderable.Right = Right ?? _initialState.Right;
    renderable.Bottom = Bottom ?? _initialState.Bottom;
    renderable.PaddingTop = PaddingTop ?? _initialState.PaddingTop;
    renderable.PaddingRight = PaddingRight ?? _initialState.PaddingRight;
    renderable.PaddingBottom = PaddingBottom ?? _initialState.PaddingBottom;
    renderable.PaddingLeft = PaddingLeft ?? _initialState.PaddingLeft;
    renderable.MarginTop = MarginTop ?? _initialState.MarginTop;
    renderable.MarginRight = MarginRight ?? _initialState.MarginRight;
    renderable.MarginBottom = MarginBottom ?? _initialState.MarginBottom;
    renderable.MarginLeft = MarginLeft ?? _initialState.MarginLeft;
    renderable.ZIndex = ZIndex ?? _initialState.ZIndex;
    renderable.Visible = Visible ?? _initialState.Visible;
    renderable.Opacity = Opacity ?? _initialState.Opacity;
    renderable.Focusable = Focusable ?? _initialState.Focusable;
  }

  public virtual void Dispose()
  {
    if (!_initialized)
      return;

    if (Parent != null)
      Parent.RemoveChild(Renderable);
    else
      App.Renderer.Root.Remove(Renderable.Id);

    Renderable.Destroy();
    App.RequestRender();
  }

  private readonly record struct CommonParameterState(
      string Id,
      FlexDirection FlexDirection,
      AlignItems AlignItems,
      JustifyContent JustifyContent,
      float FlexGrow,
      float FlexShrink,
      string Position,
      int? Top,
      int? Left,
      int? Right,
      int? Bottom,
      int PaddingTop,
      int PaddingRight,
      int PaddingBottom,
      int PaddingLeft,
      int MarginTop,
      int MarginRight,
      int MarginBottom,
      int MarginLeft,
      int ZIndex,
      bool Visible,
      float Opacity,
      bool Focusable)
  {
    public static CommonParameterState Capture(Renderable renderable)
        => new(
            renderable.Id,
            renderable.FlexDirection,
            renderable.AlignItems,
            renderable.JustifyContent,
            renderable.FlexGrow,
            renderable.FlexShrink,
            renderable.Position,
            renderable.Top,
            renderable.Left,
            renderable.Right,
            renderable.Bottom,
            renderable.PaddingTop,
            renderable.PaddingRight,
            renderable.PaddingBottom,
            renderable.PaddingLeft,
            renderable.MarginTop,
            renderable.MarginRight,
            renderable.MarginBottom,
            renderable.MarginLeft,
            renderable.ZIndex,
            renderable.Visible,
            renderable.Opacity,
            renderable.Focusable);

    public void Apply(Renderable renderable)
    {
      renderable.FlexDirection = FlexDirection;
      renderable.AlignItems = AlignItems;
      renderable.JustifyContent = JustifyContent;
      renderable.FlexGrow = FlexGrow;
      renderable.FlexShrink = FlexShrink;
      renderable.Position = Position;
      renderable.Top = Top;
      renderable.Left = Left;
      renderable.Right = Right;
      renderable.Bottom = Bottom;
      renderable.PaddingTop = PaddingTop;
      renderable.PaddingRight = PaddingRight;
      renderable.PaddingBottom = PaddingBottom;
      renderable.PaddingLeft = PaddingLeft;
      renderable.MarginTop = MarginTop;
      renderable.MarginRight = MarginRight;
      renderable.MarginBottom = MarginBottom;
      renderable.MarginLeft = MarginLeft;
      renderable.ZIndex = ZIndex;
      renderable.Visible = Visible;
      renderable.Opacity = Opacity;
      renderable.Focusable = Focusable;
    }
  }
}
