using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRewardable
{
    public int RewardIndex { get; }
    public void ActivateReward();
}
