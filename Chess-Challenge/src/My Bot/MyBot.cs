using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    const int MAX_DEPTH = 5;
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    
    public int MoveCount { get; private set; }

    public MyBot()
    {
        MoveCount = 0;
    }

    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();
        bool isWhiteBot = board.IsWhiteToMove;
        
        OrderMoves(board, allMoves);

        // Get a random move out of all the moves
        Move bestMove = allMoves[0];
        
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
        Console.WriteLine($"Total moves evaluated: {MoveCount}");
        return bestMove;
    }
    
    // Function to check if a move is a checkmate
    public bool IsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isCheckmate = board.IsInCheckmate();
        board.UndoMove(move);
        return isCheckmate;
    }
    
    // Function Negamax with alpha-beta pruning
    public int NegaMax(Board board, int depth, int alpha, int beta, bool isWhite)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return EvaluateBoard(board, isWhite);
        }
        
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
    public int EvaluateBoard(Board board, bool isWhite)
    {
        // Get the value of the pieces for white and for black
        int whiteMaterial = CountMaterial(board, true);
        int blackMaterial = CountMaterial(board, false);
        
        // Return the difference between the two values depending on the color
        int material = whiteMaterial - blackMaterial;
        return isWhite ? material : -material;
    }
    
    // Function to count the material of the board
    public int CountMaterial(Board board, bool isWhite)
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

    public void OrderMoves(Board board, Move[] moves)
    {
        int[] moveScores = new int[moves.Length];

        for (int i = 0; i < moves.Length; i++)
        {
            int moveValueGuess = 0;
            PieceType movePieceType = board.GetPiece(moves[i].StartSquare).PieceType;
            PieceType capturePieceType = board.GetPiece(moves[i].TargetSquare).PieceType;

            // If the move is a capture, add the value of the captured piece to the move value
            if (capturePieceType != PieceType.None)
            {
                moveValueGuess = 10 * pieceValues[(int)capturePieceType] - pieceValues[(int)movePieceType];
            }

            if (moves[i].IsPromotion)
            {
                moveValueGuess += pieceValues[(int)moves[i].PromotionPieceType] - pieceValues[(int)movePieceType];
            }

            // Check if the position where we're moving is attacked by an opponent pawn
            if (BitboardHelper.GetPawnAttacks(moves[i].TargetSquare, !board.IsWhiteToMove) != 0)
            {
                moveValueGuess -= pieceValues[(int)movePieceType];
            }

            // Store the move score in an array
            moveScores[i] = moveValueGuess;
        }

        // Sort the moves based on move scores (descending order)
        Sort(moves, moveScores);
    }

    public void Sort(Move[] moves, int[] moveScores)
    {
        Array.Sort(moveScores, moves, Comparer<int>.Create((a, b) => b.CompareTo(a)));
    }
}
