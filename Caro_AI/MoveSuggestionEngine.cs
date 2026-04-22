using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro_AI
{
    public class MoveSuggestionEngine
    {
        public static MoveSuggestion? SuggestMove(string[,] board, string mySymbol, string opponentSymbol)
        {
            int boardSize = board.GetLength(0);

            MoveSuggestion? winningMove = FindCriticalMove(board, mySymbol, boardSize);
            if (winningMove != null) return winningMove;

            MoveSuggestion? blockingMove = FindCriticalMove(board, opponentSymbol, boardSize);
            if (blockingMove != null) return blockingMove;

            return FindSmartHeuristicMove(board, mySymbol, opponentSymbol, boardSize);
        }

        static MoveSuggestion? FindCriticalMove(string[,] board, string symbol, int boardSize)
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (!string.IsNullOrEmpty(board[row, col])) continue;
                    board[row, col] = symbol;
                    bool isWinning = IsWinningPosition(board, row, col, symbol, boardSize);
                    board[row, col] = string.Empty;
                    if (isWinning) return new MoveSuggestion(row, col);
                }
            }

            return null;
        }

        static MoveSuggestion? FindSmartHeuristicMove(string[,] board, string mySymbol, string opponentSymbol, int boardSize)
        {
            List<MoveSuggestion> candidates = GetCandidateMoves(board, boardSize);
            int bestScore = int.MinValue;
            MoveSuggestion? bestMove = null;

            foreach (MoveSuggestion move in candidates)
            {
                board[move.Row, move.Col] = mySymbol;
                int attackScore = ScoreMove(board, move.Row, move.Col, mySymbol, boardSize);
                int defenseScore = ScoreMove(board, move.Row, move.Col, opponentSymbol, boardSize);
                int opponentBestResponse = EvaluateOpponentBestResponse(board, opponentSymbol, boardSize);
                board[move.Row, move.Col] = string.Empty;

                int totalScore = attackScore + (defenseScore * 2) - (int)(opponentBestResponse * 0.75);
                if (totalScore > bestScore)
                {
                    bestScore = totalScore;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        static int EvaluateOpponentBestResponse(string[,] board, string opponentSymbol, int boardSize)
        {
            int bestResponse = 0;
            foreach (MoveSuggestion move in GetCandidateMoves(board, boardSize))
            {
                if (!string.IsNullOrEmpty(board[move.Row, move.Col])) continue;
                int responseScore = ScoreMove(board, move.Row, move.Col, opponentSymbol, boardSize);
                if (responseScore > bestResponse) bestResponse = responseScore;
            }

            return bestResponse;
        }

        static int ScoreMove(string[,] board, int row, int col, string symbol, int boardSize)
        {
            int[][] directions =
            {
            new[] { 1, 0 },
            new[] { 0, 1 },
            new[] { 1, 1 },
            new[] { 1, -1 }
        };

            int total = 0;
            foreach (int[] dir in directions)
            {
                int leftCount = CountInDirection(board, row, col, -dir[0], -dir[1], symbol, boardSize);
                int rightCount = CountInDirection(board, row, col, dir[0], dir[1], symbol, boardSize);
                int stones = leftCount + rightCount + 1;
                int openEnds = CountOpenEnds(board, row, col, dir[0], dir[1], leftCount, rightCount, boardSize);
                total += GetPatternScore(stones, openEnds);
            }

            int center = boardSize / 2 - 1;
            int distanceToCenter = System.Math.Abs(row - center) + System.Math.Abs(col - center);
            total += boardSize - distanceToCenter;
            return total;
        }

        static int CountInDirection(string[,] board, int row, int col, int dRow, int dCol, string symbol, int boardSize)
        {
            int r = row + dRow;
            int c = col + dCol;
            int count = 0;

            while (IsInside(r, c, boardSize) && board[r, c] == symbol)
            {
                count++;
                r += dRow;
                c += dCol;
            }

            return count;
        }

        static int CountOpenEnds(string[,] board, int row, int col, int dRow, int dCol, int leftCount, int rightCount, int boardSize)
        {
            int openEnds = 0;

            int leftR = row - dRow * (leftCount + 1);
            int leftC = col - dCol * (leftCount + 1);
            if (IsInside(leftR, leftC, boardSize) && string.IsNullOrEmpty(board[leftR, leftC])) openEnds++;

            int rightR = row + dRow * (rightCount + 1);
            int rightC = col + dCol * (rightCount + 1);
            if (IsInside(rightR, rightC, boardSize) && string.IsNullOrEmpty(board[rightR, rightC])) openEnds++;

            return openEnds;
        }

        static int GetPatternScore(int stones, int openEnds)
        {
            if (stones >= 5) return 1_000_000;
            if (stones == 4 && openEnds == 2) return 200_000;
            if (stones == 4 && openEnds == 1) return 40_000;
            if (stones == 3 && openEnds == 2) return 10_000;
            if (stones == 3 && openEnds == 1) return 1_500;
            if (stones == 2 && openEnds == 2) return 400;
            if (stones == 2 && openEnds == 1) return 80;
            return 10;
        }

        static List<MoveSuggestion> GetCandidateMoves(string[,] board, int boardSize)
        {
            List<MoveSuggestion> candidates = new();

            if (!HasAnyStone(board, boardSize))
            {
                int center = boardSize / 2 - 1;
                candidates.Add(new MoveSuggestion(center, center));
                return candidates;
            }

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (!string.IsNullOrEmpty(board[row, col])) continue;
                    if (HasNeighbor(board, row, col, 2, boardSize))
                        candidates.Add(new MoveSuggestion(row, col));
                }
            }

            if (candidates.Count == 0)
            {
                int center = boardSize / 2 - 1;
                candidates.Add(new MoveSuggestion(center, center));
            }

            return candidates;
        }

        static bool HasAnyStone(string[,] board, int boardSize)
        {
            for (int row = 0; row < boardSize; row++)
                for (int col = 0; col < boardSize; col++)
                    if (!string.IsNullOrEmpty(board[row, col])) return true;
            return false;
        }

        static bool HasNeighbor(string[,] board, int row, int col, int radius, int boardSize)
        {
            for (int dr = -radius; dr <= radius; dr++)
            {
                for (int dc = -radius; dc <= radius; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr;
                    int nc = col + dc;
                    if (IsInside(nr, nc, boardSize) && !string.IsNullOrEmpty(board[nr, nc])) return true;
                }
            }

            return false;
        }

        static bool IsInside(int row, int col, int boardSize)
        {
            return row >= 0 && row < boardSize && col >= 0 && col < boardSize;
        }

        static bool IsWinningPosition(string[,] board, int row, int col, string symbol, int boardSize)
        {
            return CountLine(board, row, col, 1, 0, symbol, boardSize) >= 5
                   || CountLine(board, row, col, 0, 1, symbol, boardSize) >= 5
                   || CountLine(board, row, col, 1, 1, symbol, boardSize) >= 5
                   || CountLine(board, row, col, 1, -1, symbol, boardSize) >= 5;
        }

        static int CountLine(string[,] board, int row, int col, int dRow, int dCol, string symbol, int boardSize)
        {
            int count = 1;

            int r = row + dRow;
            int c = col + dCol;
            while (IsInside(r, c, boardSize) && board[r, c] == symbol)
            {
                count++;
                r += dRow;
                c += dCol;
            }

            r = row - dRow;
            c = col - dCol;
            while (IsInside(r, c, boardSize) && board[r, c] == symbol)
            {
                count++;
                r -= dRow;
                c -= dCol;
            }

            return count;
        }
    }
}
