using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateItemReward : MonoBehaviour, IRewardable
{
    [SerializeField] private int index;
    [SerializeField] private GameObject itemRewardPrefab;

    public int RewardIndex => index;

    public void ActivateReward()
    {
        if (itemRewardPrefab != null)
        {
            itemRewardPrefab.SetActive(true);
            itemRewardPrefab.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
