using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPiece : ChessPiece
{
    private static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
        new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };

    public override List<Vector2Int> GetCandidateMoves(ChessPiece[,] board, ChessTile[,] tiles)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var dir in Directions)
        {
            Vector2Int target = BoardPos + dir;
            if (!IsOnBoard(target)) continue;

            ChessPiece targetPiece = board[target.x, target.y];
            if (targetPiece != null && targetPiece.Color == this.Color)
                continue;

            moves.Add(target);
        }

        return moves;
    }
}
