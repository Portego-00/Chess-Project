using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        const int MAX_DEPTH = 3;

        public Move Think(Board board, Timer timer)
        {
            Move[] allMoves = board.GetLegalMoves();
            Move[] bestMoves = new Move[allMoves.Length];
            int bestMoveIndex = 0;

            int bestScore = int.MinValue;
            Move bestMove = allMoves[0];
            bool isWhiteBot = board.IsWhiteToMove;

            foreach (Move move in allMoves)
            {
                // Always play checkmate in one
                if (MoveIsCheckmate(board, move))
                {
                    return move;
                }

                board.MakeMove(move);

                int score = Minimax(board, MAX_DEPTH, int.MinValue, int.MaxValue, false, isWhiteBot);

                board.UndoMove(move);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                    bestMoves = new Move[allMoves.Length];
                    bestMoves[0] = move;
                    bestMoveIndex = 1;
                    
                }
                else if (score == bestScore)
                {
                    bestMoves[bestMoveIndex] = move;
                    bestMoveIndex++;
                }
            }
            // If there are multiple moves with the same score, pick one at random
            if (bestMoveIndex > 1)
            {
                Random random = new Random();
                int randomIndex = random.Next(bestMoveIndex);
                bestMove = bestMoves[randomIndex];
            }
            return bestMove;
        }

        int Minimax(Board board, int depth, int alpha, int beta, bool isMaximizing, bool isWhiteBot)
    {
        if (depth == 0)
        {
            return EvaluateBoard(board, isWhiteBot);
        }

        Move[] allMoves = board.GetLegalMoves();

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            foreach (Move move in allMoves)
            {
                board.MakeMove(move);

                int score = Minimax(board, depth - 1, alpha, beta, false, isWhiteBot);

                board.UndoMove(move);

                bestScore = Math.Max(score, bestScore);
                alpha = Math.Max(alpha, bestScore);

                if (beta <= alpha)
                {
                    break;
                }
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            foreach (Move move in allMoves)
            {
                board.MakeMove(move);

                int score = Minimax(board, depth - 1, alpha, beta, true, isWhiteBot);

                board.UndoMove(move);

                bestScore = Math.Min(score, bestScore);
                beta = Math.Min(beta, bestScore);

                if (beta <= alpha)
                {
                    break;
                }
            }

            return bestScore;
        }
    }

        int EvaluateBoard(Board board, bool isWhiteBot)
        {
            int whiteMaterial = CountMaterial(board, pieceValues, true);
            int blackMaterial = CountMaterial(board, pieceValues, false);

            // Simple evaluation: difference in material
            if (isWhiteBot)
            {
                return whiteMaterial - blackMaterial;
            }
            else
            {
                return blackMaterial - whiteMaterial;
            }
        }

        bool MoveIsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }

        static int CountMaterial(Board board, int[] pieceValues, bool isWhite)
        {
            int score = 0;
            for (int i = 0; i < 64; i++)
            {
                Square square = new Square(i);
                Piece piece = board.GetPiece(square);
                if (piece != null && piece.IsWhite == isWhite)
                {
                    score += pieceValues[(int)piece.PieceType];
                }
            }
            return score;
        }
    }
}