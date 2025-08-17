using System;
using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private int matchedObjectIndex;
    IPuzzleTrigger currentInteractable;

    //이벤트 구독으로 컨트롤러에게 알림
    public event Action<PuzzleTrigger, bool> OnPuzzleStateChanged;

    public int Index => index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IPuzzleTrigger>(out var iTrigger))
        {
            if (iTrigger.ObjectIndex == matchedObjectIndex)
            {
                currentInteractable = iTrigger;
                iTrigger.TriggerEnter();

                OnPuzzleStateChanged?.Invoke(this, true);
            }
        }
    }
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.TryGetComponent<IPuzzleTrigger>(out var iTrigger))
    //     {
    //         if (iTrigger != currentInteractable)
    //             return;
    //         currentInteractable?.TriggerExit();
    //
    //         OnPuzzleStateChanged?.Invoke(this, false);
    //     }
    // }
}