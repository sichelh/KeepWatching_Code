using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    public override List<Vector2Int> GetCandidateMoves(ChessPiece[,] board, ChessTile[,] tiles)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int direction = (Color == PieceColor.White) ? 1 : -1;
        Vector2Int forward = BoardPos + new Vector2Int(0, direction);

        if (IsOnBoard(forward) && board[forward.x, forward.y] == null)
        {
            moves.Add(forward);

            Vector2Int doubleForward = BoardPos + new Vector2Int(0, 2 * direction);
            if ((Color == PieceColor.White && BoardPos.y == 1) || (Color == PieceColor.Black && BoardPos.y == 6))
            {
                if (board[doubleForward.x, doubleForward.y] == null)
                    moves.Add(doubleForward);
            }
        }

        Vector2Int[] diagonalOffsets = {
            new Vector2Int(-1, direction),
            new Vector2Int(1, direction)
        };

        foreach (var offset in diagonalOffsets)
        {
            Vector2Int diagonal = BoardPos + offset;
            if (IsOnBoard(diagonal))
            {
                ChessPiece target = board[diagonal.x, diagonal.y];
                if (target != null && target.Color != Color)
                {
                    moves.Add(diagonal);
                }
            }
        }
        return moves;
    }
}
