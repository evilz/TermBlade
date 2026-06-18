using System;
using System.Collections.Generic;
using System.Linq;
using TermBlade.Core.Ansi;

namespace TermBlade.Core.Text
{
  /// <summary>
  /// Read-only styled text store — C# port of the TS/Zig <c>TextBuffer</c>.
  /// Holds a sequence of <see cref="StyledChunk"/>s and exposes plain-text queries.
  /// </summary>
  public sealed class TextBuffer : IDisposable
  {
    private readonly List<StyledChunk> _chunks = new();
    private bool _disposed;

    /// <summary>
    /// Gets or sets the default fg.
    /// </summary>
    public Rgba? DefaultFg { get; set; }
    /// <summary>
    /// Gets or sets the default bg.
    /// </summary>
    public Rgba? DefaultBg { get; set; }
    /// <summary>
    /// Gets or sets the default attributes.
    /// </summary>
    public TextAttributes? DefaultAttributes { get; set; }

    private string PlainText => string.Concat(_chunks.Select(c => c.Text));

    /// <summary>Number of non-newline characters.</summary>
    public int Length => PlainText.Replace("\n", "").Length;
    /// <summary>
    /// Gets the byte size.
    /// </summary>
    public int ByteSize => System.Text.Encoding.UTF8.GetByteCount(PlainText);

    /// <summary>
    /// Gets the create.
    /// </summary>
    public static TextBuffer Create() => new TextBuffer();

    /// <summary>
    /// Set text.
    /// </summary>
    /// <param name="text">The text value.</param>
    public void SetText(string text)
    {
      Guard();
      _chunks.Clear();
      _chunks.Add(new StyledChunk(text, DefaultFg, DefaultBg,
                                  DefaultAttributes ?? TextAttributes.None));
    }

    /// <summary>
    /// Set styled text.
    /// </summary>
    /// <param name="text">The text value.</param>
    public void SetStyledText(StyledText text)
    {
      Guard();
      _chunks.Clear();
      _chunks.AddRange(text.Chunks);
    }

    /// <summary>
    /// Get plain text.
    /// </summary>
    public string GetPlainText() { Guard(); return PlainText; }

    /// <summary>
    /// Get chunks.
    /// </summary>
    public List<StyledChunk> GetChunks() { Guard(); return new List<StyledChunk>(_chunks); }

    /// <summary>
    /// Set default fg.
    /// </summary>
    public void SetDefaultFg(Rgba? fg) { Guard(); DefaultFg = fg; }
    /// <summary>
    /// Set default bg.
    /// </summary>
    public void SetDefaultBg(Rgba? bg) { Guard(); DefaultBg = bg; }
    /// <summary>
    /// Set default attributes.
    /// </summary>
    public void SetDefaultAttributes(TextAttributes? a) { Guard(); DefaultAttributes = a; }

    private void Guard()
    {
      if (_disposed) throw new ObjectDisposedException(nameof(TextBuffer));
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    public void Dispose() { _disposed = true; }
  }
}
