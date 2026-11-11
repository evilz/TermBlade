using System.Globalization;

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
  /// <exception cref="ArgumentOutOfRangeException">The value is negative or not finite.</exception>
  public static LayoutDimension Fixed(float v)
  {
    ValidateValue(v, nameof(v));
    return new(DimType.Fixed, v);
  }
  /// <summary>
  /// Gets the percent.
  /// </summary>
  /// <param name="v">The v value.</param>
  /// <exception cref="ArgumentOutOfRangeException">The value is negative or not finite.</exception>
  public static LayoutDimension Percent(float v)
  {
    ValidateValue(v, nameof(v));
    return new(DimType.Percent, v);
  }

  /// <summary>
  /// Parses a fixed or percentage dimension using invariant numeric syntax.
  /// </summary>
  /// <param name="value">The value value.</param>
  /// <exception cref="FormatException">A string is not <c>auto</c> or a valid invariant number.</exception>
  /// <exception cref="ArgumentOutOfRangeException">A numeric value is negative or not finite.</exception>
  public static LayoutDimension Parse(object? value)
  {
    return value switch
    {
      null => Auto,
      int i => CreateParsedValue(i, DimType.Fixed),
      float f => CreateParsedValue(f, DimType.Fixed),
      double d => CreateParsedValue((float)d, DimType.Fixed),
      string s when s.Equals("auto", StringComparison.OrdinalIgnoreCase) => Auto,
      string s => ParseString(s),
      _ => Auto,
    };
  }

  private static LayoutDimension ParseString(string value)
  {
    var text = value.Trim();
    if (text.Equals("auto", StringComparison.OrdinalIgnoreCase))
      return Auto;

    bool isPercent = text.EndsWith('%');
    var number = isPercent ? text[..^1].Trim() : text;
    if (!float.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
      throw new FormatException($"'{value}' is not a valid layout dimension.");

    return CreateParsedValue(parsed, isPercent ? DimType.Percent : DimType.Fixed);
  }

  private static LayoutDimension CreateParsedValue(float value, DimType type)
  {
    ValidateValue(value, nameof(value));
    return new(type, value);
  }

  private static void ValidateValue(float value, string paramName)
  {
    if (!float.IsFinite(value) || value < 0)
      throw new ArgumentOutOfRangeException(paramName, value, "Layout dimensions must be finite and non-negative.");
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
