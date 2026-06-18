namespace TermBlade.Core.Input;

/// <summary>
/// Represents key event.
/// </summary>
public class KeyEvent
{
  /// <summary>
  /// Gets or sets the name.
  /// </summary>
  public string Name { get; init; } = "";
  /// <summary>
  /// Gets or sets the key.
  /// </summary>
  public string Key { get; init; } = "";
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
  /// <summary>
  /// Gets or sets the meta.
  /// </summary>
  public bool Meta { get; init; }
  /// <summary>
  /// Gets or sets the char.
  /// </summary>
  public char? Char { get; init; }
  /// <summary>
  /// Gets or sets the default prevented.
  /// </summary>
  public bool DefaultPrevented { get; private set; }

  /// <summary>
  /// Prevent default.
  /// </summary>
  public void PreventDefault()
  {
    DefaultPrevented = true;
  }
}
