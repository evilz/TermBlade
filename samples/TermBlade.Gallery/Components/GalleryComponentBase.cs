using Microsoft.AspNetCore.Components;
using TermBlade.Core.Input;
using TermBlade.Razor.Hosting;

namespace TermBlade.Gallery.Components;

public abstract class GalleryComponentBase : ComponentBase, IDisposable
{
  [Inject] protected TermBladeAppContext App { get; set; } = null!;

  private readonly List<Action<object?>> _keypressHandlers = [];

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

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;

    foreach (var handler in _keypressHandlers)
      App.KeyEvents.Off("keypress", handler);
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}
