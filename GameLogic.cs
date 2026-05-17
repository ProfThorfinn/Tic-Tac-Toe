namespace TicTacToe
{
    public enum Difficulty { Easy, Medium, Hard }

    public static class GameLogic
    {
        // ── All 8 winning lines (cell indices) ───────────────
        private static readonly int[][] Lines =
        {
            new[] { 0, 1, 2 },   // row 0
            new[] { 3, 4, 5 },   // row 1
            new[] { 6, 7, 8 },   // row 2
            new[] { 0, 3, 6 },   // col 0
            new[] { 1, 4, 7 },   // col 1
            new[] { 2, 5, 8 },   // col 2
            new[] { 0, 4, 8 },   // diagonal ↘
            new[] { 2, 4, 6 },   // diagonal ↙
        };

        /// <summary>Returns "X", "O", "draw", or null (game ongoing).</summary>
        public static string? CheckWinner(string?[] board)
        {
            foreach (var l in Lines)
                if (board[l[0]] != null &&
                    board[l[0]] == board[l[1]] &&
                    board[l[1]] == board[l[2]])
                    return board[l[0]];

            foreach (var c in board)
                if (c == null) return null;

            return "draw";
        }

        /// <summary>Returns the 3 winning cell indices, or null.</summary>
        public static int[]? GetWinningLine(string?[] board)
        {
            foreach (var l in Lines)
                if (board[l[0]] != null &&
                    board[l[0]] == board[l[1]] &&
                    board[l[1]] == board[l[2]])
                    return l;
            return null;
        }

        // ── MinMax with Alpha-Beta Pruning ────────────────────
        // O = maximiser (+10), X = minimiser (−10)
        public static int Minimax(string?[] board, bool isMaximising,
                                   int alpha = int.MinValue, int beta = int.MaxValue)
        {
            var result = CheckWinner(board);
            if (result == "O") return 10;
            if (result == "X") return -10;
            if (result == "draw") return 0;

            if (isMaximising)
            {
                int best = int.MinValue;
                for (int i = 0; i < 9; i++)
                {
                    if (board[i] != null) continue;
                    board[i] = "O";
                    best = Math.Max(best, Minimax(board, false, alpha, beta));
                    board[i] = null;
                    alpha = Math.Max(alpha, best);
                    if (beta <= alpha) break;          // β cut-off
                }
                return best;
            }
            else
            {
                int best = int.MaxValue;
                for (int i = 0; i < 9; i++)
                {
                    if (board[i] != null) continue;
                    board[i] = "X";
                    best = Math.Min(best, Minimax(board, true, alpha, beta));
                    board[i] = null;
                    beta = Math.Min(beta, best);
                    if (beta <= alpha) break;          // α cut-off
                }
                return best;
            }
        }

        /// <summary>Returns the best cell index for O using full MinMax.</summary>
        public static int BestMinimaxMove(string?[] board)
        {
            int bestScore = int.MinValue, bestMove = -1;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != null) continue;
                board[i] = "O";
                int score = Minimax(board, false);
                board[i] = null;
                if (score > bestScore) { bestScore = score; bestMove = i; }
            }
            return bestMove;
        }

        private static readonly Random Rng = new();

        public static int RandomMove(string?[] board)
        {
            var empty = Enumerable.Range(0, 9).Where(i => board[i] == null).ToList();
            return empty.Count > 0 ? empty[Rng.Next(empty.Count)] : -1;
        }

        /// <summary>
        /// Easy   → 100 % random
        /// Medium → 60 % MinMax + 40 % random
        /// Hard   → 100 % MinMax (unbeatable)
        /// </summary>
        public static int AiMove(string?[] board, Difficulty diff)
        {
            return diff switch
            {
                Difficulty.Easy => RandomMove(board),
                Difficulty.Medium => Rng.NextDouble() < 0.60
                                         ? BestMinimaxMove(board)
                                         : RandomMove(board),
                _ => BestMinimaxMove(board),   // Hard
            };
        }
    }
}