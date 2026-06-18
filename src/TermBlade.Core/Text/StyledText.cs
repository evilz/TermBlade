using System.Collections.Generic;
using TermBlade.Core.Ansi;

namespace TermBlade.Core.Text
{
  /// <summary>A single styled run of text.</summary>
  public sealed class StyledChunk
  {
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string Text { get; set; }
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
    /// Gets or sets the link.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Styled chunk.
    /// </summary>
    /// <param name="text">The text value.</param>
    /// <param name="fg">The fg value.</param>
    /// <param name="bg">The bg value.</param>
    /// <param name="attrs">The attrs value.</param>
    /// <param name="link">The link value.</param>
    public StyledChunk(string text, Rgba? fg = null, Rgba? bg = null,
                       TextAttributes attrs = TextAttributes.None, string? link = null)
    {
      Text = text; Fg = fg; Bg = bg; Attributes = attrs; Link = link;
    }
  }

  /// <summary>A sequence of styled text chunks — the C# equivalent of the TS StyledText type.</summary>
  public sealed class StyledText
  {
    /// <summary>
    /// Gets the chunks.
    /// </summary>
    public List<StyledChunk> Chunks { get; } = new();

    /// <summary>
    /// From string.
    /// </summary>
    /// <param name="text">The text value.</param>
    public static StyledText FromString(string text) =>
        new StyledText { Chunks = { new StyledChunk(text) } };

    /// <summary>
    /// To plain text.
    /// </summary>
    public string ToPlainText()
    {
      var sb = new System.Text.StringBuilder();
      foreach (var c in Chunks) sb.Append(c.Text);
      return sb.ToString();
    }

    /// <summary>
    /// Gets the length.
    /// </summary>
    public int Length => ToPlainText().Replace("\n", "").Length;
  }
}
