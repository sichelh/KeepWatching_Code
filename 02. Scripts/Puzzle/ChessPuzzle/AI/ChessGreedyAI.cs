using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessGreedyAI : MonoBehaviour
{
    [SerializeField] private ChessBoardManager board;

    public void MakeGreedyMove()
    {
        List<(ChessPiece, Vector2Int, int)> candidates = new();

        foreach (var piece in board.GetAllPieces())
        {
            if (piece.Color != PieceColor.Black) continue;

            foreach (var move in board.GetLegalMoves(piece))
            {
                ChessPiece target = board.BoardState[move.x, move.y];
                int value = target == null ? 0 : GetPieceValue(target);
                candidates.Add((piece, move, value));
            }
        }

        if (candidates.Count > 0)
        {
            int maxValue = candidates.Max(c => c.Item3);
            var bestMoves = candidates.Where(c => c.Item3 == maxValue).ToList();
            var (piece, move, _) = bestMoves[Random.Range(0, bestMoves.Count)];
            board.MoveRequested(piece, move);
        }
        else
        {

        }

        if (board.IsCheckmate(PieceColor.White))
        {
            ChessTurnManager.Instance.FinishGame();
        }
    }

    private int GetPieceValue(ChessPiece piece)
    {
        return piece.Type switch
        {
            PieceType.Pawn => 1,
            PieceType.Knight or PieceType.Bishop => 3,
            PieceType.Rook => 5,
            PieceType.Queen => 9,
            PieceType.King => 100,
            _ => 0
        };
    }
}
