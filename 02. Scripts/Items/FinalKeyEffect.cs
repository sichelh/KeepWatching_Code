using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FinalKeyEffect : IItemEffect
{
    private LayerMask doorLayerMask;

    public FinalKeyEffect()
    {
        doorLayerMask = LayerMask.GetMask("ClearDoor");
    }

    public void Use(out bool canUse)
    {
        if (!IsLookingAtFinalDoor())
        {
            canUse = false;
            return;
        }

        Transform interactionPlace = GameObject.Find("ClearDoor")?.transform.Find("InteractionArea");
        if (interactionPlace != null)
        {
            MeshRenderer mesh = interactionPlace.GetComponent<MeshRenderer>();
            if (mesh != null)
                mesh.enabled = true;
        }
        canUse = true;
        SceneManager.LoadScene("ClearScene");
    }

    private bool IsLookingAtFinalDoor()
    {
        Camera camera = Camera.main;
        Vector3 origin = camera.transform.position;
        Vector3 direction = camera.transform.forward;

        return Physics.Raycast(origin, direction, out _, 3f, doorLayerMask);
    }
}
