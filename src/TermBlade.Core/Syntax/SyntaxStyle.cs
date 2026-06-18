using System;
using System.Collections.Generic;
using TermBlade.Core.Ansi;

namespace TermBlade.Core.Syntax
{
  /// <summary>A single named style entry in a <see cref="SyntaxStyle"/>.</summary>
  public sealed class StyleDefinition
  {
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// Gets or sets the fg.
    /// </summary>
    public Rgba? Fg { get; set; }
    /// <summary>
    /// Gets or sets the bg.
    /// </summary>
    public Rgba? Bg { get; set; }
    /// <summary>
    /// Gets or sets the attributes.
    /// </summary>
    public TextAttributes Attributes { get; set; }
    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    public int Priority { get; set; }
  }

  /// <summary>
  /// Registry of named syntax styles. C# port of the Zig/TS <c>SyntaxStyle</c>.
  /// </summary>
  public sealed class SyntaxStyle
  {
    private readonly Dictionary<string, StyleDefinition> _styles =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the create.
    /// </summary>
    public static SyntaxStyle Create() => new SyntaxStyle();

    /// <summary>
    /// Register style.
    /// </summary>
    /// <param name="definition">The definition value.</param>
    public void RegisterStyle(StyleDefinition definition)
        => _styles[definition.Name] = definition;

    /// <summary>
    /// Get style.
    /// </summary>
    /// <param name="name">The name value.</param>
    public StyleDefinition? GetStyle(string name)
        => _styles.TryGetValue(name, out var s) ? s : null;

    /// <summary>
    /// Gets the all styles.
    /// </summary>
    public IEnumerable<StyleDefinition> AllStyles => _styles.Values;

    /// <summary>
    /// Merge two style definitions: override wins on non-null fields,
    /// attributes are OR-ed, priority takes the maximum.
    /// </summary>
    public static StyleDefinition MergeStyles(StyleDefinition baseStyle,
                                              StyleDefinition overrideStyle) =>
        new StyleDefinition
        {
          Name = overrideStyle.Name,
          Fg = overrideStyle.Fg ?? baseStyle.Fg,
          Bg = overrideStyle.Bg ?? baseStyle.Bg,
          Attributes = overrideStyle.Attributes | baseStyle.Attributes,
          Priority = Math.Max(baseStyle.Priority, overrideStyle.Priority),
        };
  }
}
