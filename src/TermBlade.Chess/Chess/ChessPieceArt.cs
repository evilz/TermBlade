namespace TermBlade.Chess.Chess;

/// <summary>
/// Multi-line ASCII art for chess pieces.
/// Each piece is rendered as 4 rows × 5 characters, padded to a given width.
/// Art style based on the Extended display mode from chess-tui.
/// </summary>
public static class ChessPieceArt
{
  // 5-character-wide art rows for each piece type (4 rows per piece).
  // Empty square: 4 rows of spaces.
  private static readonly string[] EmptyRows = ["     ", "     ", "     ", "     "];

  private static readonly string[] PawnRows =
  [
      "     ",
        " ▝█▘ ",
        " ▟█▙ ",
        " ▔▔▔ ",
    ];

  private static readonly string[] KnightRows =
  [
      "  ▖▗ ",
        "▗▇▟█▌",
        " ▟█▛ ",
        "▝▀▀▀▘",
    ];

  private static readonly string[] BishopRows =
  [
      " ▄▁▗ ",
        " ██▟ ",
        " ▟█▙ ",
        "▝▀▀▀▘",
    ];

  private static readonly string[] RookRows =
  [
      "▄ ▄ ▄",
        "█████",
        " ███ ",
        "▀▀▀▀▀",
    ];

  private static readonly string[] QueenRows =
  [
      "▂ ▄ ▂",
        "▜▙█▟▛",
        " ▜█▛ ",
        "▝▀▀▀▘",
    ];

  private static readonly string[] KingRows =
  [
      " ▂╋▂ ",
        "▜███▛",
        " ▜█▛ ",
        "▝▀▀▀▘",
    ];

  /// <summary>
  /// Returns 4 art rows for the given piece, each padded with one space on each side
  /// so the total width is 7 characters (matching the default SquareW=7).
  /// </summary>
  public static string[] GetRows(ChessPiece piece, int squareWidth = 7)
  {
    var art = piece.Type switch
    {
      PieceType.Pawn => PawnRows,
      PieceType.Knight => KnightRows,
      PieceType.Bishop => BishopRows,
      PieceType.Rook => RookRows,
      PieceType.Queen => QueenRows,
      PieceType.King => KingRows,
      _ => EmptyRows,
    };

    // Pad each 5-char art line to squareWidth (default 7: 1 space each side).
    var pad = squareWidth - 5;
    if (pad <= 0)
      return art;

    var left = pad / 2;
    var right = pad - left;
    var leftPad = new string(' ', left);
    var rightPad = new string(' ', right);

    return
    [
        leftPad + art[0] + rightPad,
            leftPad + art[1] + rightPad,
            leftPad + art[2] + rightPad,
            leftPad + art[3] + rightPad,
        ];
  }
}
