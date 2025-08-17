using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardEffect : IItemEffect
{
    private Transform boardSpawnPivot;
    private LayerMask altarLayerMask;

    public ChessBoardEffect()
    {
        boardSpawnPivot = GameObject.Find("ChessboardPlace")?.transform.Find("ChessBoardPivot");
        altarLayerMask = LayerMask.GetMask("Altar");
    }

    public void Use(out bool canUse)
    {
        if (!IsLookingAtAltar())
        {
            canUse = false;
            return;
        }

        ChessBoardManager boardMgr = GameObject.FindObjectOfType<ChessBoardManager>();
        if (boardMgr == null)
        {
            canUse = false;
            return;
        }

        boardMgr.gameObject.SetActive(true);
        boardMgr.Setup(boardSpawnPivot);

        Transform interactionPlace = GameObject.Find("ChessboardPlace")?.transform.Find("InteractionPlace");
        if (interactionPlace != null)
        {
            MeshRenderer mesh = interactionPlace.GetComponent<MeshRenderer>();
            if (mesh != null)
                mesh.enabled = true;
        }

        canUse = true;
    }

    private bool IsLookingAtAltar()
    {
        Camera  camera    = Camera.main;
        Vector3 origin    = camera.transform.position;
        Vector3 direction = camera.transform.forward;

        if (Physics.Raycast(origin, direction, out var hit, 3f, altarLayerMask))
        {
            return true;
        }

        return false;
    }
}