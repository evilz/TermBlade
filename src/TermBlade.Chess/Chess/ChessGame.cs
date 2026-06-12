namespace TermBlade.Chess.Chess;

/// <summary>Types of chess pieces.</summary>
public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }

/// <summary>Color of a chess piece or player.</summary>
public enum PieceColor { None, White, Black }

/// <summary>Immutable value representing a single chess piece (or empty square).</summary>
public readonly record struct ChessPiece(PieceType Type, PieceColor Color)
{
  /// <summary>Represents an empty square.</summary>
  public static readonly ChessPiece Empty = new(PieceType.None, PieceColor.None);

  /// <summary>True when this square is empty.</summary>
  public bool IsEmpty => Type == PieceType.None;

  /// <summary>Single-letter identifier used in move notation.</summary>
  public string Notation => Type switch
  {
    PieceType.King => "K",
    PieceType.Queen => "Q",
    PieceType.Rook => "R",
    PieceType.Bishop => "B",
    PieceType.Knight => "N",
    PieceType.Pawn => "",
    _ => " ",
  };

  /// <summary>Chess emoji symbol for this piece (e.g. ♔ for white king, ♚ for black king).</summary>
  public string Symbol => (Type, Color) switch
  {
    (PieceType.King, PieceColor.White) => "♔",
    (PieceType.Queen, PieceColor.White) => "♕",
    (PieceType.Rook, PieceColor.White) => "♖",
    (PieceType.Bishop, PieceColor.White) => "♗",
    (PieceType.Knight, PieceColor.White) => "♘",
    (PieceType.Pawn, PieceColor.White) => "♙",
    (PieceType.King, PieceColor.Black) => "♚",
    (PieceType.Queen, PieceColor.Black) => "♛",
    (PieceType.Rook, PieceColor.Black) => "♜",
    (PieceType.Bishop, PieceColor.Black) => "♝",
    (PieceType.Knight, PieceColor.Black) => "♞",
    (PieceType.Pawn, PieceColor.Black) => "♟",
    _ => " ",
  };
}

/// <summary>
/// Manages board state, move generation, capture tracking and move history for a two-player chess game.
/// Supports all standard piece movements and pawn promotion (auto-queen).
/// Omits en passant and castling for simplicity.
/// </summary>
public sealed class ChessGame
{
  private readonly ChessPiece[,] _board = new ChessPiece[8, 8];
  private readonly List<string> _history = [];
  private readonly List<ChessPiece> _whiteCaptured = [];
  private readonly List<ChessPiece> _blackCaptured = [];

  public PieceColor CurrentTurn { get; private set; } = PieceColor.White;
  public bool IsGameOver { get; private set; }
  public string StatusMessage { get; private set; } = string.Empty;
  public IReadOnlyList<string> History => _history;
  public IReadOnlyList<ChessPiece> WhiteCaptured => _whiteCaptured;
  public IReadOnlyList<ChessPiece> BlackCaptured => _blackCaptured;

  public ChessGame() => Reset();

  /// <summary>Resets the board to the standard starting position.</summary>
  public void Reset()
  {
    for (var r = 0; r < 8; r++)
      for (var c = 0; c < 8; c++)
        _board[r, c] = ChessPiece.Empty;

    _history.Clear();
    _whiteCaptured.Clear();
    _blackCaptured.Clear();
    CurrentTurn = PieceColor.White;
    IsGameOver = false;
    StatusMessage = string.Empty;

    // Black back rank (row 7 = rank 8)
    PlaceBackRank(7, PieceColor.Black);
    for (var c = 0; c < 8; c++)
      _board[6, c] = new ChessPiece(PieceType.Pawn, PieceColor.Black);

    // White back rank (row 0 = rank 1)
    PlaceBackRank(0, PieceColor.White);
    for (var c = 0; c < 8; c++)
      _board[1, c] = new ChessPiece(PieceType.Pawn, PieceColor.White);
  }

  private void PlaceBackRank(int row, PieceColor color)
  {
    _board[row, 0] = new ChessPiece(PieceType.Rook, color);
    _board[row, 1] = new ChessPiece(PieceType.Knight, color);
    _board[row, 2] = new ChessPiece(PieceType.Bishop, color);
    _board[row, 3] = new ChessPiece(PieceType.Queen, color);
    _board[row, 4] = new ChessPiece(PieceType.King, color);
    _board[row, 5] = new ChessPiece(PieceType.Bishop, color);
    _board[row, 6] = new ChessPiece(PieceType.Knight, color);
    _board[row, 7] = new ChessPiece(PieceType.Rook, color);
  }

  /// <summary>Returns the piece at the given board position (row 0 = rank 1, col 0 = file A).</summary>
  public ChessPiece GetPiece(int row, int col) => _board[row, col];

  /// <summary>Returns all squares the piece at (row, col) can legally move to.</summary>
  public List<(int Row, int Col)> GetLegalMoves(int row, int col)
  {
    var piece = _board[row, col];
    if (piece.IsEmpty || piece.Color != CurrentTurn)
      return [];

    var pseudo = GetPseudoMoves(row, col);
    var legal = new List<(int, int)>(pseudo.Count);

    foreach (var (tr, tc) in pseudo)
    {
      var saved = ApplyMove(row, col, tr, tc);
      if (!IsInCheck(CurrentTurn))
        legal.Add((tr, tc));
      UndoMove(row, col, tr, tc, saved);
    }

    return legal;
  }

  /// <summary>
  /// Attempts to move the piece from (fromRow,fromCol) to (toRow,toCol).
  /// Returns true and updates game state when the move is legal; returns false otherwise.
  /// </summary>
  public bool TryMove(int fromRow, int fromCol, int toRow, int toCol)
  {
    if (IsGameOver)
      return false;

    var legal = GetLegalMoves(fromRow, fromCol);
    if (!legal.Contains((toRow, toCol)))
      return false;

    var captured = _board[toRow, toCol];
    var piece = _board[fromRow, fromCol];

    _board[toRow, toCol] = piece;
    _board[fromRow, fromCol] = ChessPiece.Empty;

    // Auto-promote pawns to queen
    if (piece.Type == PieceType.Pawn)
    {
      var promotionRank = piece.Color == PieceColor.White ? 7 : 0;
      if (toRow == promotionRank)
        _board[toRow, toCol] = new ChessPiece(PieceType.Queen, piece.Color);
    }

    if (!captured.IsEmpty)
    {
      if (captured.Color == PieceColor.White)
        _whiteCaptured.Add(captured);
      else
        _blackCaptured.Add(captured);
    }

    // Build move notation
    var fromFile = (char)('a' + fromCol);
    var fromRank = (char)('1' + fromRow);
    var toFile = (char)('a' + toCol);
    var toRank = (char)('1' + toRow);
    var captureChar = captured.IsEmpty ? "-" : "x";
    _history.Add($"{piece.Symbol}{fromFile}{fromRank}{captureChar}{toFile}{toRank}");

    CurrentTurn = Opponent(CurrentTurn);

    // Detect check/checkmate/stalemate
    UpdateGameStatus();
    return true;
  }

  private void UpdateGameStatus()
  {
    var inCheck = IsInCheck(CurrentTurn);
    var hasMoves = HasAnyLegalMove(CurrentTurn);

    if (!hasMoves)
    {
      IsGameOver = true;
      StatusMessage = inCheck ? $"{Opponent(CurrentTurn)} wins by checkmate!" : "Draw by stalemate!";
    }
    else if (inCheck)
    {
      StatusMessage = $"{CurrentTurn} is in check!";
    }
    else
    {
      StatusMessage = string.Empty;
    }
  }

  private bool HasAnyLegalMove(PieceColor color)
  {
    for (var r = 0; r < 8; r++)
      for (var c = 0; c < 8; c++)
        if (_board[r, c].Color == color && GetLegalMoves(r, c).Count > 0)
          return true;
    return false;
  }

  private bool IsInCheck(PieceColor color)
  {
    var (kr, kc) = FindKing(color);
    if (kr < 0)
      return true;

    var opp = Opponent(color);
    for (var r = 0; r < 8; r++)
      for (var c = 0; c < 8; c++)
        if (_board[r, c].Color == opp && GetPseudoMoves(r, c).Contains((kr, kc)))
          return true;
    return false;
  }

  private (int Row, int Col) FindKing(PieceColor color)
  {
    for (var r = 0; r < 8; r++)
      for (var c = 0; c < 8; c++)
        if (_board[r, c].Type == PieceType.King && _board[r, c].Color == color)
          return (r, c);
    return (-1, -1);
  }

  // Temporarily apply a move (returns captured piece for undo).
  private ChessPiece ApplyMove(int fr, int fc, int tr, int tc)
  {
    var captured = _board[tr, tc];
    _board[tr, tc] = _board[fr, fc];
    _board[fr, fc] = ChessPiece.Empty;
    return captured;
  }

  private void UndoMove(int fr, int fc, int tr, int tc, ChessPiece captured)
  {
    _board[fr, fc] = _board[tr, tc];
    _board[tr, tc] = captured;
  }

  private List<(int, int)> GetPseudoMoves(int row, int col)
  {
    var piece = _board[row, col];
    var moves = new List<(int, int)>();

    switch (piece.Type)
    {
      case PieceType.Pawn: AddPawnMoves(row, col, piece.Color, moves); break;
      case PieceType.Knight: AddKnightMoves(row, col, piece.Color, moves); break;
      case PieceType.Bishop: AddSlidingMoves(row, col, piece.Color, moves, DiagonalDirs); break;
      case PieceType.Rook: AddSlidingMoves(row, col, piece.Color, moves, CardinalDirs); break;
      case PieceType.Queen: AddSlidingMoves(row, col, piece.Color, moves, AllDirs); break;
      case PieceType.King: AddKingMoves(row, col, piece.Color, moves); break;
    }

    return moves;
  }

  private static readonly (int Dr, int Dc)[] DiagonalDirs = [(-1, -1), (-1, 1), (1, -1), (1, 1)];
  private static readonly (int Dr, int Dc)[] CardinalDirs = [(-1, 0), (1, 0), (0, -1), (0, 1)];
  private static readonly (int Dr, int Dc)[] AllDirs = [(-1, -1), (-1, 1), (1, -1), (1, 1), (-1, 0), (1, 0), (0, -1), (0, 1)];

  private void AddPawnMoves(int row, int col, PieceColor color, List<(int, int)> moves)
  {
    var dir = color == PieceColor.White ? 1 : -1;
    var startRow = color == PieceColor.White ? 1 : 6;
    var nr = row + dir;

    if (InBounds(nr, col) && _board[nr, col].IsEmpty)
    {
      moves.Add((nr, col));
      var nr2 = row + 2 * dir;
      if (row == startRow && _board[nr2, col].IsEmpty)
        moves.Add((nr2, col));
    }

    for (var dc = -1; dc <= 1; dc += 2)
    {
      var nc = col + dc;
      if (InBounds(nr, nc) && !_board[nr, nc].IsEmpty && _board[nr, nc].Color != color)
        moves.Add((nr, nc));
    }
  }

  private void AddKnightMoves(int row, int col, PieceColor color, List<(int, int)> moves)
  {
    (int Dr, int Dc)[] offsets = [(-2, -1), (-2, 1), (-1, -2), (-1, 2), (1, -2), (1, 2), (2, -1), (2, 1)];
    foreach (var (dr, dc) in offsets)
    {
      var (nr, nc) = (row + dr, col + dc);
      if (InBounds(nr, nc) && (_board[nr, nc].IsEmpty || _board[nr, nc].Color != color))
        moves.Add((nr, nc));
    }
  }

  private void AddSlidingMoves(int row, int col, PieceColor color, List<(int, int)> moves, (int Dr, int Dc)[] dirs)
  {
    foreach (var (dr, dc) in dirs)
    {
      var (nr, nc) = (row + dr, col + dc);
      while (InBounds(nr, nc))
      {
        if (_board[nr, nc].IsEmpty)
        {
          moves.Add((nr, nc));
        }
        else
        {
          if (_board[nr, nc].Color != color)
            moves.Add((nr, nc));
          break;
        }
        nr += dr;
        nc += dc;
      }
    }
  }

  private void AddKingMoves(int row, int col, PieceColor color, List<(int, int)> moves)
  {
    foreach (var (dr, dc) in AllDirs)
    {
      var (nr, nc) = (row + dr, col + dc);
      if (InBounds(nr, nc) && (_board[nr, nc].IsEmpty || _board[nr, nc].Color != color))
        moves.Add((nr, nc));
    }
  }

  private static bool InBounds(int r, int c) => (uint)r < 8 && (uint)c < 8;

  private static PieceColor Opponent(PieceColor color)
      => color == PieceColor.White ? PieceColor.Black : PieceColor.White;
}
