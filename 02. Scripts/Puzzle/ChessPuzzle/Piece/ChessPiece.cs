using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChessPiece : MonoBehaviour, IInteractable
{
    public PieceType  Type     { get; private set; }
    public PieceColor Color    { get; private set; }
    public Vector2Int BoardPos { get; private set; }

    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material highlightMaterial;

    [SerializeField] private PieceHighlighter highlighter;

    [SerializeField] private Renderer rend;
    private Material originalMaterial;

    private ChessBoardManager chessBoardManager;
    public Outline     Outline       { get; }
    public IVisibility VisibilityObj { get; private set; }

    #region Initialize

    public void Initialize(PieceType type, PieceColor color, Vector2Int startPos, ChessBoardManager chessBoardManager)
    {
        Type = type;
        Color = color;
        SetBoardPos(startPos);

        rend = GetComponentInChildren<Renderer>();
        originalMaterial = (color == PieceColor.White) ? whiteMaterial : blackMaterial;

        if (rend != null)
            rend.material = originalMaterial;

        this.chessBoardManager = chessBoardManager;
        transform.position = chessBoardManager.BoardToWorld(startPos);
        // transform.position = FindObjectOfType<ChessBoardManager>().BoardToWorld(startPos);
    }

    #endregion

    #region Move To

    public void MoveTo(Vector2Int newPos)
    {
        SetBoardPos(newPos);
        // Vector3 targetPos = FindObjectOfType<ChessBoardManager>().BoardToWorld(newPos);
        Vector3 targetPos = chessBoardManager.BoardToWorld(newPos);
        transform.position = targetPos;
        highlighter.StopHighlight(rend, originalMaterial);
    }

    #endregion

    #region Hightlight Piece

    public void Select()
    {
        if (highlighter != null)
            highlighter.StartHighlight(rend);
    }

    public void Deselect()
    {
        if (highlighter != null)
            highlighter.StopHighlight(rend, originalMaterial);
    }

    #endregion

    protected List<Vector2Int> GetLinearMoves(ChessPiece[,] board, Vector2Int[] directions)
    {
        List<Vector2Int> moves = new();
        foreach (var dir in directions)
        {
            Vector2Int next = BoardPos + dir;
            while (IsOnBoard(next))
            {
                if (board[next.x, next.y] == null)
                {
                    moves.Add(next);
                }
                else
                {
                    if (board[next.x, next.y].Color != this.Color)
                        moves.Add(next);
                    break;
                }

                next += dir;
            }
        }

        return moves;
    }

    protected bool IsOnBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }

    public void SetBoardPos(Vector2Int pos)
    {
        BoardPos = pos;
    }

    public virtual List<Vector2Int> GetCandidateMoves(ChessPiece[,] board, ChessTile[,] tiles)
    {
        return new List<Vector2Int>();
    }


    public void Execute()
    {
        if (chessBoardManager.SelectedPiece == this)
        {
            Exit();
        }
        else
        {
            chessBoardManager.SelectPiece(this);
        }
    }

    public void Exit()
    {
        chessBoardManager.DeselectPiece();
    }
}