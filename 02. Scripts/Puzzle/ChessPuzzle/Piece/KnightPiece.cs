using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightPiece : ChessPiece
{
    public override List<Vector2Int> GetCandidateMoves(ChessPiece[,] board, ChessTile[,] tiles)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] offsets = {
            new Vector2Int(+1, +2), new Vector2Int(+1, -2),
            new Vector2Int(-1, +2), new Vector2Int(-1, -2),
            new Vector2Int(+2, +1), new Vector2Int(+2, -1),
            new Vector2Int(-2, +1), new Vector2Int(-2, -1),
        };

        foreach (var offset in offsets)
        {
            Vector2Int pos = BoardPos + offset;
            if (IsOnBoard(pos) && (board[pos.x, pos.y] == null || board[pos.x, pos.y].Color != this.Color))
            {
                moves.Add(pos);
            }
        }
        return moves;
    }
}
