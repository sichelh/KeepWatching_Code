using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClearSceneEvent : MonoBehaviour
{
    public GameObject GhostPrefab;

    public void GetEvent(int @eventCode)
    {
        switch (@eventCode)
        {
            case 0:
                {
                    GhostPrefab.SetActive(true);
                    break;
                }
            case 1:
                {
                    // 꼬마아이 웃기
                    GhostPrefab.transform.Find("SK_Head").
                    GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(54, 100f);
                    break;
                }
        }
    }
}
