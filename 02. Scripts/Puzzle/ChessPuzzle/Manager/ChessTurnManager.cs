using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState { WaitingForPlayer, AIThinking, GameOver }

public class ChessTurnManager : MonoBehaviour
{
    public static ChessTurnManager Instance { get; private set; }

    public PieceColor CurrentTurn {  get; private set; } = PieceColor.White;
    public TurnState CurrentState { get; private set; } = TurnState.WaitingForPlayer;

    [SerializeField] private ChessBoardManager boardManager;
    [SerializeField] private ChessGreedyAI ai;
    [SerializeField] private GameObject rewardItemPrefab;
    [SerializeField] private Transform rewardDropPoint;
    //[SerializeField] private ChessAI chessAI;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    #region End Turn
    public void TryEndTurn()
    {
        if (CurrentState == TurnState.GameOver) return;

        PieceColor enemy = (CurrentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;

        if (boardManager.IsCheckmate(enemy))
        {
            //Debug.Log($"{enemy} is Checkmated. {CurrentTurn} Wins!");
            CurrentState = TurnState.GameOver;
            return;
        }

        CurrentTurn = enemy;
        CurrentState = (enemy == PieceColor.White) ? TurnState.WaitingForPlayer : TurnState.AIThinking;


        if (CurrentState == TurnState.AIThinking)
            StartCoroutine(HandleAITurn());
    }
    #endregion

    #region AI Turn
    private IEnumerator HandleAITurn()
    {
        yield return new WaitForSeconds(0.5f);

        //chessAI.MakeAIMove();
        ai.MakeGreedyMove();

        yield return new WaitForSeconds(0.3f);
        TryEndTurn();
    }
    #endregion

    public bool CanPlayerAct()
    {
        return CurrentTurn == PieceColor.White && CurrentState == TurnState.WaitingForPlayer;
    }

    public void OnPlayerMoved()
    {
        TryEndTurn();
    }

    public void FinishGame()
    {
        CurrentState = TurnState.GameOver;

        if (CurrentTurn == PieceColor.White)
        {
            HandlePlayerVictory();
        }
        else
        {
            HandlePlayerDefeat();
        }
    }

    private void HandlePlayerVictory()
    {
        Vector3 dropPosition = rewardDropPoint.position + Vector3.up * 2f;
        Instantiate(rewardItemPrefab, dropPosition, Quaternion.identity);
        AudioManager.Instance.PlaySFX(SFX.ChurchBell);
    }

    private void HandlePlayerDefeat()
    {
        boardManager.ClearBoard();
        boardManager.SetupPuzzle();

        CurrentTurn = PieceColor.White;
        CurrentState = TurnState.WaitingForPlayer;
    }
}
