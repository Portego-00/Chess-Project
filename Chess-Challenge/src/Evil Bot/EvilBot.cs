using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        const int MAX_DEPTH = 4;
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        
        public int MoveCount { get; private set; }

        public EvilBot()
        {
            MoveCount = 0;
        }

        public Move Think(Board board, Timer timer)
        {
            MoveCount = 0;
            
            Move[] allMoves = board.GetLegalMoves();
            bool isWhiteBot = board.IsWhiteToMove;

            // Get a random move out of all the moves
            Move bestMove = allMoves[new Random().Next(allMoves.Length)];
            
            int bestScore = int.MinValue;
            
            foreach (Move move in allMoves)
            {
                if (IsCheckmate(board, move))
                {
                    return move;
                }
                
                // Evaluate the board for all the moves
                board.MakeMove(move);
                int score = -NegaMax(board, MAX_DEPTH - 1, int.MinValue + 1, int.MaxValue - 1, !isWhiteBot);
                board.UndoMove(move);
                
                // If the score is better than the current best move, update the best move
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            Console.WriteLine($"Total moves evaluated (Evil): {MoveCount}");
            return bestMove;
        }
        
        // Function to check if a move is a checkmate
        bool IsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isCheckmate = board.IsInCheckmate();
            board.UndoMove(move);
            return isCheckmate;
        }
        
        // Function Negamax with alpha-beta pruning
        int NegaMax(Board board, int depth, int alpha, int beta, bool isWhite)
        {
            if (depth == 0) { return EvaluateBoard(board, isWhite); }
            
            int bestScore = int.MinValue;
            Move[] allMoves = board.GetLegalMoves();

            foreach (Move move in allMoves)
            {
                MoveCount++;
                board.MakeMove(move);
                int score = -NegaMax(board, depth - 1, -beta, -alpha, !isWhite);
                board.UndoMove(move);
                
                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, score);
                
                if (alpha >= beta) { break; }
            }
            
            return bestScore;
        }
        
        
        // Function to evaluate the board
        int EvaluateBoard(Board board, bool isWhite)
        {
            // Get the value of the pieces for white and for black
            int whiteMaterial = CountMaterial(board, true);
            int blackMaterial = CountMaterial(board, false);
            
            // Return the difference between the two values depending on the color
            int material = whiteMaterial - blackMaterial;
            return isWhite ? material : -material;
        }
        
        // Function to count the material of the board
        int CountMaterial(Board board, bool isWhite)
        {
            int material = 0;
            for (int i = 0; i < 64; i++)
            {
                Square square = new Square(i);
                Piece piece = board.GetPiece(square);
                if (piece.IsWhite == isWhite)
                {
                    material += pieceValues[(int)piece.PieceType];
                }
            }
            return material;
        }
    }
}