using TermBlade.Chess.Chess;

namespace TermBlade.Tests;

public class ChessGameTests
{
  [Fact]
  public void GetLegalMoves_IncludesBasicPawnAndKnightMoves()
  {
    var game = new ChessGame();

    var pawnMoves = game.GetLegalMoves(1, 4);
    var knightMoves = game.GetLegalMoves(0, 1);
    var bishopMoves = game.GetLegalMoves(0, 2);

    Assert.Contains((2, 4), pawnMoves);
    Assert.Contains((3, 4), pawnMoves);
    Assert.Contains((2, 0), knightMoves);
    Assert.Contains((2, 2), knightMoves);
    Assert.Empty(bishopMoves);
  }

  [Fact]
  public void TryMove_RejectsIllegalMove()
  {
    var game = new ChessGame();

    var moved = game.TryMove(1, 0, 1, 1);

    Assert.False(moved);
    Assert.Equal(PieceColor.White, game.CurrentTurn);
    Assert.Equal(PieceType.Pawn, game.GetPiece(1, 0).Type);
    Assert.Equal(PieceType.Pawn, game.GetPiece(1, 1).Type);
  }

  [Fact]
  public void TryMove_FoolsMateSetsCheckmateStatus()
  {
    var game = new ChessGame();

    Assert.True(game.TryMove(1, 5, 2, 5)); // f2-f3
    Assert.True(game.TryMove(6, 4, 4, 4)); // e7-e5
    Assert.True(game.TryMove(1, 6, 3, 6)); // g2-g4
    Assert.True(game.TryMove(7, 3, 3, 7)); // Qd8-h4#

    Assert.True(game.IsGameOver);
    Assert.Equal("Black wins by checkmate!", game.StatusMessage);
  }

  [Fact]
  public void TryMove_PromotesPawnToQueenOnBackRank()
  {
    var game = new ChessGame();

    Assert.True(game.TryMove(1, 6, 3, 6)); // g2-g4
    Assert.True(game.TryMove(6, 7, 4, 7)); // h7-h5
    Assert.True(game.TryMove(3, 6, 4, 7)); // g4xh5
    Assert.True(game.TryMove(6, 0, 5, 0)); // a7-a6
    Assert.True(game.TryMove(4, 7, 5, 7)); // h5-h6
    Assert.True(game.TryMove(5, 0, 4, 0)); // a6-a5
    Assert.True(game.TryMove(5, 7, 6, 7)); // h6-h7
    Assert.True(game.TryMove(6, 1, 5, 1)); // b7-b6
    Assert.True(game.TryMove(6, 7, 7, 6)); // h7xg8=Q

    var promoted = game.GetPiece(7, 6);
    Assert.Equal(PieceColor.White, promoted.Color);
    Assert.Equal(PieceType.Queen, promoted.Type);
  }
}
