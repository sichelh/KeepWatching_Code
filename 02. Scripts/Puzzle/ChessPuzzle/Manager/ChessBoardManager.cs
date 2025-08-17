using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardManager : MonoBehaviour
{
    public ChessPuzzleData puzzleData;

    private ChessTile[,] tiles = new ChessTile[8, 8];
    private ChessPiece[,] board = new ChessPiece[8, 8];

    public ChessPiece[,] BoardState => board;
    public ChessTile[,]  Tiles      => tiles;

    private ChessPiece selectedPiece;
    public ChessPiece SelectedPiece => selectedPiece;

    private int currentSolutionIndex = 0;

    [SerializeField] private Transform boardOrigin;
    [SerializeField] private float pieceYOffset = 0f;

    [SerializeField] private GameObject kingPrefab;
    [SerializeField] private GameObject queenPrefab;
    [SerializeField] private GameObject bishopPrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject rookPrefab;
    [SerializeField] private GameObject pawnPrefab;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Material whiteTileMaterial;
    [SerializeField] private Material blackTileMaterial;

    [SerializeField] private PromotionUI promotionUI;
    [SerializeField] private ChessNoticeUI noticeUI;

    #region Setup

    public void GenerateTiles()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3    spawnPos = boardOrigin.position + new Vector3(x, 0, y);
                GameObject tileGO   = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);

                ChessTile tile    = tileGO.GetComponent<ChessTile>();
                bool      isWhite = (x + y) % 2 != 0;
                Material  baseMat = isWhite ? whiteTileMaterial : blackTileMaterial;

                tile.Initialize(new Vector2Int(x, y), baseMat);
                tiles[x, y] = tile;
            }
        }
    }

    public void SetupPuzzle()
    {
        ClearBoard();
        foreach (PieceInfo info in puzzleData.initialPieces)
        {
            GameObject prefab = GetPrefabFor(info.type) ?? pawnPrefab;
            GameObject go     = Instantiate(prefab, BoardToWorld(info.boardPos), Quaternion.identity);
            ChessPiece piece  = go.GetComponent<ChessPiece>();
            piece.Initialize(info.type, info.color, info.boardPos, this);
            board[info.boardPos.x, info.boardPos.y] = piece;
            tiles[info.boardPos.x, info.boardPos.y].occupyingPiece = piece;
        }
    }

    public void Setup(Transform boardOrigin)
    {
        this.boardOrigin = boardOrigin;
        GenerateTiles();
        SetupPuzzle();
        noticeUI.ShowHint();
    }

    private void ResetPuzzle()
    {
        currentSolutionIndex = 0;
        SetupPuzzle();
        AudioManager.Instance.PlaySFX(SFX.FearGhost);
        noticeUI.ShowWrongMoveAlarm();
    }

    #endregion

    #region Clear Board

    public void ClearBoard()
    {
        for (int x = 0; x < 8; x++)
        for (int y = 0; y < 8; y++)
        {
            if (board[x, y] != null)
            {
                Destroy(board[x, y].gameObject);
                board[x, y] = null;
            }

            tiles[x, y].occupyingPiece = null;
        }
    }

    #endregion

    #region Board to world

    public Vector3 BoardToWorld(Vector2Int bpos)
    {
        return tiles[bpos.x, bpos.y].transform.position + Vector3.up * pieceYOffset;
    }

    #endregion

    #region Get Chess Piece Prefabs

    GameObject GetPrefabFor(PieceType type)
    {
        switch (type)
        {
            case PieceType.King:
                return kingPrefab;
            case PieceType.Queen:
                return queenPrefab;
            case PieceType.Bishop:
                return bishopPrefab;
            case PieceType.Knight:
                return knightPrefab;
            case PieceType.Rook:
                return rookPrefab;
            case PieceType.Pawn:
                return pawnPrefab;
            default:
                return null;
        }
    }

    #endregion

    #region Select & Deselect Piece

    public void SelectPiece(ChessPiece piece)
    {
        if (!ChessTurnManager.Instance.CanPlayerAct())
            return;

        if (piece.Color != ChessTurnManager.Instance.CurrentTurn)
        {
            return;
        }

        if (selectedPiece != null)
            selectedPiece.Deselect();

        ClearHighlights();

        selectedPiece = piece;
        selectedPiece.Select();

        var validMoves = GetLegalMoves(piece);
        foreach (var move in validMoves)
            tiles[move.x, move.y].SetHighlight(true);
    }

    public void DeselectPiece()
    {
        if (selectedPiece != null)
            selectedPiece.Deselect();

        selectedPiece = null;
        ClearHighlights();
    }

    public void ClearHighlights()
    {
        foreach (var tile in tiles)
            tile.ResetHighlight();
    }

    #endregion

    #region Move

    public void MoveRequested(ChessPiece piece, Vector2Int target)
    {
        Vector2Int from     = piece.BoardPos;
        ChessPiece captured = board[target.x, target.y];

        if (captured != null && captured.Color != piece.Color)
        {
            Destroy(captured.gameObject);
        }

        board[from.x, from.y] = null;
        board[target.x, target.y] = piece;
        piece.MoveTo(target);

        if (piece.Type == PieceType.Pawn)
        {
            if ((piece.Color == PieceColor.White && target.y == 7) || (piece.Color == PieceColor.Black && target.y == 0))
            {
                HandlePromotion(piece);
                return;
            }
        }

        PieceColor enemyColor = (piece.Color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        if (IsCheckmate(enemyColor))
        {
            ChessTurnManager.Instance.FinishGame();
            return;
        }
    }

    public void TryMoveSelectedPieceTo(ChessTile targetTile)
    {
        if (!ChessTurnManager.Instance.CanPlayerAct()) return;
        if (selectedPiece == null) return;

        List<Vector2Int> validMoves = GetLegalMoves(selectedPiece);
        if (!validMoves.Contains(targetTile.BoardPos)) return;

        if (currentSolutionIndex < puzzleData.solutionMoves.Count)
        {
            Move expected = puzzleData.solutionMoves[currentSolutionIndex];

            bool isCorrect =
                selectedPiece.Type == expected.pieceType &&
                selectedPiece.BoardPos == expected.from &&
                targetTile.BoardPos == expected.to;

            if (!isCorrect)
            {
                ResetPuzzle();
                ClearHighlights();
                return;
            }
        }
        else
        {
            ResetPuzzle();
            ClearHighlights();
            return;
        }

        MoveRequested(selectedPiece, targetTile.BoardPos);
        selectedPiece = null;

        ClearHighlights();
        currentSolutionIndex++;
        ChessTurnManager.Instance.OnPlayerMoved();
    }

    #endregion

    #region Promotion

    public void HandlePromotion(ChessPiece pawn)
    {
        if (pawn.Color == PieceColor.White)
        {
            promotionUI.Show(pawn, promoteTo =>
            {
                PromotePawn(pawn, promoteTo);

                PieceColor enemyColor = (pawn.Color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
                if (IsCheckmate(enemyColor))
                {
                    ChessTurnManager.Instance.FinishGame();
                }
            });
        }
        else
        {
            PromotePawn(pawn, PieceType.Queen);

            PieceColor enemyColor = PieceColor.White;
            if (IsCheckmate(enemyColor))
            {
                ChessTurnManager.Instance.FinishGame();
            }
        }
    }

    public void PromotePawn(ChessPiece pawn, PieceType promoteTo)
    {
        Vector2Int pos = pawn.BoardPos;
        Destroy(pawn.gameObject);

        GameObject prefab        = GetPrefabFor(promoteTo);
        GameObject promoted      = Instantiate(prefab, BoardToWorld(pos), Quaternion.identity);
        ChessPiece promotedPiece = promoted.GetComponent<ChessPiece>();
        promotedPiece.Initialize(promoteTo, pawn.Color, pos, this);

        board[pos.x, pos.y] = promotedPiece;
        tiles[pos.x, pos.y].occupyingPiece = promotedPiece;

        promotedPiece.SetBoardPos(pos);
    }

    #endregion

    #region IsInCheck

    public bool IsInCheck(PieceColor color)
    {
        Vector2Int kingPos = Vector2Int.zero;
        foreach (var piece in board)
        {
            if (piece != null && piece.Type == PieceType.King && piece.Color == color)
            {
                kingPos = piece.BoardPos;
                break;
            }
        }

        if (kingPos == Vector2Int.zero)
        {
            return false;
        }

        PieceColor enemyColor = (color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        foreach (var piece in board)
        {
            if (piece != null && piece.Color == enemyColor && piece.Type != PieceType.King)
            {
                List<Vector2Int> moves = piece.GetCandidateMoves(board, tiles);

                if (moves.Contains(kingPos))
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion

    #region Checkmate Check

    public bool IsCheckmate(PieceColor color)
    {
        if (!IsInCheck(color))
            return false;

        foreach (ChessPiece piece in board)
        {
            if (piece == null || piece.Color != color)
                continue;

            List<Vector2Int> moves = GetLegalMoves(piece);
            if (moves.Count > 0)
                return false;
        }

        return true;
    }

    #endregion

    #region Legal Move Filter

    public List<Vector2Int> GetLegalMoves(ChessPiece piece)
    {
        List<Vector2Int> candidates = piece.GetCandidateMoves(board, tiles);
        List<Vector2Int> legalMoves = new();

        Vector2Int originalPos = piece.BoardPos;

        foreach (var move in candidates)
        {
            ChessPiece captured = board[move.x, move.y];
            board[originalPos.x, originalPos.y] = null;
            board[move.x, move.y] = piece;
            piece.SetBoardPos(move);

            bool isInCheck = IsInCheck(piece.Color);

            board[originalPos.x, originalPos.y] = piece;
            board[move.x, move.y] = captured;
            piece.SetBoardPos(originalPos);

            if (!isInCheck)
                legalMoves.Add(move);
        }

        return legalMoves;
    }

    #endregion

    public bool HasSelectedPiece()
    {
        return selectedPiece != null;
    }

    public List<ChessPiece> GetAllPieces()
    {
        List<ChessPiece> result = new List<ChessPiece>();
        foreach (var piece in board)
            if (piece != null)
                result.Add(piece);
        return result;
    }

    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }
}