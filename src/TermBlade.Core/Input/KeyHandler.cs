using TermBlade.Core.Events;

namespace TermBlade.Core.Input;

/// <summary>
/// Represents key handler.
/// </summary>
public class KeyHandler : EventEmitter
{
  /// <summary>
  /// On.
  /// </summary>
  /// <param name="eventName">The eventName value.</param>
  /// <param name="handler">The handler value.</param>
  public void On(string eventName, Action<KeyEvent> handler)
  {
    On(eventName, (object? data) =>
    {
      if (data is KeyEvent key) handler(key);
    });
  }

  /// <summary>
  /// On.
  /// </summary>
  /// <param name="eventName">The eventName value.</param>
  /// <param name="handler">The handler value.</param>
  public void On(string eventName, Action<MouseEvent> handler)
  {
    On(eventName, (object? data) =>
    {
      if (data is MouseEvent mouse) handler(mouse);
    });
  }

  /// <summary>
  /// Gets the emit key.
  /// </summary>
  /// <param name="key">The key value.</param>
  public void EmitKey(KeyEvent key) => Emit("keypress", key);
  /// <summary>
  /// Gets the emit mouse.
  /// </summary>
  /// <param name="mouse">The mouse value.</param>
  public void EmitMouse(MouseEvent mouse) => Emit("mouse", mouse);
}
