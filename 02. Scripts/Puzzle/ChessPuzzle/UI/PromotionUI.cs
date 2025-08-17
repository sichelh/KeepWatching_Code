using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [SerializeField] private Button queenButton;
    [SerializeField] private Button rookButton;
    [SerializeField] private Button bishipButton;
    [SerializeField] private Button knightButton;

    private Action<PieceType> onSelect;

    private void Awake()
    {
        gameObject.SetActive(false);

        queenButton.onClick.AddListener(() => Select(PieceType.Queen));
        rookButton.onClick.AddListener(() => Select(PieceType.Rook));
        bishipButton.onClick.AddListener(() => Select(PieceType.Bishop));
        knightButton.onClick.AddListener(() => Select(PieceType.Knight));
    }

    public void Show(ChessPiece pawn, Action<PieceType> onSelectCallback)
    {
        this.onSelect = onSelectCallback;
        gameObject.SetActive(true);
    }

    private void Select(PieceType chosen)
    {
        onSelect?.Invoke(chosen);
        gameObject.SetActive(false);
    }
}
