using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChessTile : MonoBehaviour
{
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material highlightMat;

    public Vector2Int BoardPos;
    public ChessPiece occupyingPiece;

    public void Initialize(Vector2Int pos, Material mat)
    {
        BoardPos = pos;
        defaultMat = mat;

        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.material = defaultMat;
    }

    public void SetHighlight(bool active)
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = active ? highlightMat : defaultMat;
        }
    }

    public void ResetHighlight()
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = defaultMat;
        }
    }
}
