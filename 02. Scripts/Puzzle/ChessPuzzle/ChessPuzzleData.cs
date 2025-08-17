using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ChessPuzzle/Data")]
public class ChessPuzzleData : ScriptableObject
{
    public PieceInfo[] initialPieces;

    public List<Move> solutionMoves;

}

[System.Serializable]
public struct PieceInfo
{
    public PieceType type;
    public PieceColor color;
    public Vector2Int boardPos;
}

[System.Serializable]
public struct Move
{
    public PieceType pieceType;
    public Vector2Int from;
    public Vector2Int to;
}

public enum PieceType { King, Queen, Rook, Bishop, Knight, Pawn }
public enum PieceColor { White, Black }