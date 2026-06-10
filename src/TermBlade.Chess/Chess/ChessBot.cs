namespace TermBlade.Chess.Chess;

/// <summary>
/// Simple in-process chess bot. Picks a random legal move for the current player,
/// preferring captures with 70% probability when any are available.
/// </summary>
public static class ChessBot
{
    private static readonly Random Rng = new();

    /// <summary>
    /// Picks the best move for the current player using a simple random strategy
    /// with a bias toward captures. Returns null when there are no legal moves.
    /// </summary>
    public static (int FromRow, int FromCol, int ToRow, int ToCol)? PickMove(ChessGame game)
    {
        var allMoves = new List<(int fr, int fc, int tr, int tc)>();
        var captures = new List<(int fr, int fc, int tr, int tc)>();

        for (var r = 0; r < 8; r++)
        for (var c = 0; c < 8; c++)
        {
            var piece = game.GetPiece(r, c);
            if (piece.IsEmpty || piece.Color != game.CurrentTurn) continue;

            foreach (var (tr, tc) in game.GetLegalMoves(r, c))
            {
                allMoves.Add((r, c, tr, tc));
                if (!game.GetPiece(tr, tc).IsEmpty)
                    captures.Add((r, c, tr, tc));
            }
        }

        if (allMoves.Count == 0) return null;

        // Prefer captures 70% of the time when available
        var pool = captures.Count > 0 && Rng.NextDouble() < 0.7 ? captures : allMoves;
        var pick = pool[Rng.Next(pool.Count)];
        return pick;
    }
}
