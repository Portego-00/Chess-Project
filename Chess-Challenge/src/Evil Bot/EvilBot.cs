using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        const int MAX_DEPTH = 3;
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        public Move Think(Board board, Timer timer)
        {
            Move[] allMoves = board.GetLegalMoves();

            bool isWhiteBot = board.IsWhiteToMove;

            // Get a random move out of all the moves
            Move bestMove = allMoves[new Random().Next(allMoves.Length)];

            foreach (Move move in allMoves)
            {
                if (IsCheckmate(board, move))
                {
                    return move;
                }
            }

            return bestMove;
        }

        bool IsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isCheckmate = board.IsInCheckmate();
            board.UndoMove(move);
            return isCheckmate;
        }
    }
}