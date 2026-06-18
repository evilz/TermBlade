namespace TermBlade.Core.Input;

/// <summary>
/// Defines mouse button values.
/// </summary>
public enum MouseButton { None, Left, Middle, Right, WheelUp, WheelDown }

/// <summary>
/// Represents mouse event.
/// </summary>
public class MouseEvent
{
  /// <summary>
  /// Gets or sets the x.
  /// </summary>
  public int X { get; init; }
  /// <summary>
  /// Gets or sets the y.
  /// </summary>
  public int Y { get; init; }
  /// <summary>
  /// Gets or sets the button.
  /// </summary>
  public MouseButton Button { get; init; }
  /// <summary>
  /// Gets or sets the pressed.
  /// </summary>
  public bool Pressed { get; init; }
  /// <summary>
  /// Gets or sets the ctrl.
  /// </summary>
  public bool Ctrl { get; init; }
  /// <summary>
  /// Gets or sets the alt.
  /// </summary>
  public bool Alt { get; init; }
  /// <summary>
  /// Gets or sets the shift.
  /// </summary>
  public bool Shift { get; init; }
}
