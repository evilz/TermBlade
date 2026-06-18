namespace TermBlade.Core.Layout;

/// <summary>
/// Represents layout dimension.
/// </summary>
public readonly struct LayoutDimension
{
  /// <summary>
  /// Defines dim type values.
  /// </summary>
  public enum DimType { Fixed, Auto, Percent }

  /// <summary>
  /// Gets the type.
  /// </summary>
  public DimType Type { get; }
  /// <summary>
  /// Gets the value.
  /// </summary>
  public float Value { get; }

  private LayoutDimension(DimType type, float value) { Type = type; Value = value; }

  /// <summary>
  /// Gets the auto.
  /// </summary>
  public static LayoutDimension Auto => new(DimType.Auto, 0);
  /// <summary>
  /// Gets the fixed.
  /// </summary>
  /// <param name="v">The v value.</param>
  public static LayoutDimension Fixed(float v) => new(DimType.Fixed, v);
  /// <summary>
  /// Gets the percent.
  /// </summary>
  /// <param name="v">The v value.</param>
  public static LayoutDimension Percent(float v) => new(DimType.Percent, v);

  /// <summary>
  /// Parse.
  /// </summary>
  /// <param name="value">The value value.</param>
  public static LayoutDimension Parse(object? value)
  {
    return value switch
    {
      null => Auto,
      int i => Fixed(i),
      float f => Fixed(f),
      double d => Fixed((float)d),
      string s when s.Equals("auto", StringComparison.OrdinalIgnoreCase) => Auto,
      string s when s.EndsWith('%') && float.TryParse(s[..^1], out var pct) => Percent(pct),
      string s when float.TryParse(s, out var n) => Fixed(n),
      _ => Auto,
    };
  }

  /// <summary>
  /// Resolve.
  /// </summary>
  /// <param name="containerSize">The containerSize value.</param>
  public float Resolve(float containerSize)
  {
    return Type switch
    {
      DimType.Fixed => Value,
      DimType.Percent => containerSize * Value / 100f,
      _ => containerSize,
    };
  }
}
