using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleTriggerController : MonoBehaviour
{
    // Trigger들 index 연결
    [SerializeField] private List<PuzzleTrigger> puzzleTriggers;
    private Dictionary<int, List<(PuzzleTrigger, bool)>> puzzleStatesByIndex = new();

    // reward와 index 연결
    [SerializeField] private List<MonoBehaviour> rewardObjects;
    private Dictionary<int, IRewardable> rewardables = new();

    private void Start()
    {
        foreach (var trigger in puzzleTriggers)
        {
            if (!puzzleStatesByIndex.TryGetValue(trigger.Index, out var list))
            {
                list = new List<(PuzzleTrigger, bool)>();
            }

            trigger.OnPuzzleStateChanged += PuzzleStateChanged;
            list.Add((trigger, false));
            puzzleStatesByIndex[trigger.Index] = list;
        }

        foreach (var obj in rewardObjects)
        {
            if(obj is IRewardable reward)
            {
                rewardables[reward.RewardIndex] = reward;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var trigger in puzzleTriggers)
        {
            trigger.OnPuzzleStateChanged -= PuzzleStateChanged;
        }
    }

    private void PuzzleStateChanged(PuzzleTrigger trigger, bool isActivated)
    {
        int findIndex = puzzleStatesByIndex[trigger.Index].FindIndex(x => x.Item1 == trigger);
        if (findIndex != -1)
        {
            puzzleStatesByIndex[trigger.Index][findIndex] = (trigger, isActivated);
        }

        CheckPuzzle(trigger.Index);
    }


    private void CheckPuzzle(int index)
    {
        if (puzzleStatesByIndex.TryGetValue(index, out var list))
        {
            if (list.Any(x => x.Item2 == false))
                return;

            ActivatePuzzleReward(index);
        }
    }

    private void ActivatePuzzleReward(int index)
    {
        Debug.Log($"퍼즐 해결! 보상 제공 {index}");
        if(rewardables.TryGetValue(index, out var reward))
        {
            reward.ActivateReward();
            AudioManager.Instance.PlaySFX(SFX.ChurchBell);
        }
    }
}