using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPuzzle : PuzzleController
{
    [SerializeField] private ChessBoardManager board;
    public override bool IsSolved { get; protected set; }

    public override void StartPuzzle()
    {
        IsSolved = false;
    }

    public override void ResetPuzzle()
    {
        board.SetupPuzzle();
        IsSolved = false;
    }

    public void EvaluateBoardState()
    {
        ChessPiece blackKing = FindBlackKing();
        if (blackKing == null)
        {
            Complete();
            return;
        }

        bool isInCheck = IsKingInCheck(blackKing);
        bool hasNoMoves = !HasLegalMoves(PieceColor.Black);

        if (isInCheck && hasNoMoves)
        {
            //Debug.Log("체크메이트 성공");
            Complete();
        }
    }

    ChessPiece FindBlackKing()
    {
        foreach (var piece in board.GetAllPieces())
            if (piece.Type == PieceType.King && piece.Color == PieceColor.Black)
                return piece;
        return null;
    }

    bool IsKingInCheck(ChessPiece king)
    {
        foreach (var piece in board.GetAllPieces())
        {
            if (piece.Color == PieceColor.White)
            {
                var moves = board.GetLegalMoves(piece);
                if (moves.Contains(king.BoardPos))
                    return true;
            }
        }
        return false;
    }

    bool HasLegalMoves(PieceColor color)
    {
        foreach (var piece in board.GetAllPieces())
        {
            if (piece.Color == color)
            {
                var moves = board.GetLegalMoves(piece);
                if (moves.Count > 0)
                    return true;
            }
        }
        return false;
    }
}
