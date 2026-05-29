using Microsoft.AspNetCore.Components;
using TermBlade.Core.Input;
using TermBlade.Razor.Hosting;

namespace TermBlade.Razor.Samples.Components;

public abstract class SampleComponentBase : ComponentBase, IDisposable
{
  [Inject] protected TermBladeAppContext App { get; set; } = null!;

  private readonly List<Action<object?>> _keypressHandlers = [];
  private readonly List<Action<object?>> _mouseHandlers = [];
  private readonly List<EventHandler<TermBlade.Core.Rendering.ResizeEventArgs>> _resizeHandlers = [];
  private readonly List<Timer> _timers = [];

  protected void SetBackground(string color) => App.Renderer.SetBackgroundColor(color);

  protected void Exit() => App.Renderer.Destroy();

  protected void RegisterKeypress(Action<KeyEvent> handler)
  {
    Action<object?> wrapped = data =>
    {
      if (data is KeyEvent key)
        handler(key);
    };

    App.KeyEvents.On("keypress", wrapped);
    _keypressHandlers.Add(wrapped);
  }

  protected void RegisterMouse(Action<MouseEvent> handler)
  {
    Action<object?> wrapped = data =>
    {
      if (data is MouseEvent mouse)
        handler(mouse);
    };

    App.KeyEvents.On("mouse", wrapped);
    _mouseHandlers.Add(wrapped);
  }

  protected void RegisterResize(EventHandler<TermBlade.Core.Rendering.ResizeEventArgs> handler)
  {
    App.Renderer.Resize += handler;
    _resizeHandlers.Add(handler);
  }

  protected Timer CreateTimer(TimerCallback callback, TimeSpan period)
  {
    var timer = new Timer(callback, null, TimeSpan.Zero, period);
    _timers.Add(timer);
    return timer;
  }

  protected void RegisterExitKeys()
  {
    RegisterKeypress(key =>
    {
      if (key.Name is "q" or "escape")
      {
        key.PreventDefault();
        Exit();
      }
    });
  }

  public virtual void Dispose()
  {
    foreach (var handler in _keypressHandlers)
      App.KeyEvents.Off("keypress", handler);

    foreach (var handler in _mouseHandlers)
      App.KeyEvents.Off("mouse", handler);

    foreach (var handler in _resizeHandlers)
      App.Renderer.Resize -= handler;

    foreach (var timer in _timers)
      timer.Dispose();
  }
}
