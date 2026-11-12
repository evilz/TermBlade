namespace TermBlade.Core.Buffer;

/// <summary>
/// Keeps plain text cells from sending terminal control characters to output streams.
/// </summary>
internal static class TerminalTextSafety
{
  internal const int ReplacementCodepoint = 0xFFFD;

  internal static int ReplaceControlCodepoint(int codepoint)
    => codepoint is >= 0 and <= 0x1F or >= 0x7F and <= 0x9F
      ? ReplacementCodepoint
      : codepoint;

  internal static int NormalizeCellCodepoint(int codepoint)
    => codepoint == 0 ? 0 : ReplaceControlCodepoint(codepoint);
}
