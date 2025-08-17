using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookPiece : ChessPiece
{
    public override List<Vector2Int> GetCandidateMoves(ChessPiece[,] board, ChessTile[,] tiles)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        return GetLinearMoves(board, directions);
    }
}
