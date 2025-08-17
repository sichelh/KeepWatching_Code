using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenPiece : ChessPiece
{
    public override List<Vector2Int> GetCandidateMoves(ChessPiece[,] board, ChessTile[,] tiles)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        return GetLinearMoves(board, directions);
    }
}