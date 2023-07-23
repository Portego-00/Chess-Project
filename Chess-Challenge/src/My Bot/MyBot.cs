using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    const int MAX_DEPTH = 3;
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

    public Move Think(Board board, Timer timer)
    {
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
            int score = EvaluateBoard(board, pieceValues, isWhiteBot);
            board.UndoMove(move);
            
            // If the score is better than the current best move, update the best move
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

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
    
    
    // Function to evaluate the board
    int EvaluateBoard(Board board, int[] piecesValue, bool isWhite)
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
            if (piece != null && piece.IsWhite == isWhite)
            {
                material += pieceValues[(int)piece.PieceType];
            }
        }
        return material;
    }
}
