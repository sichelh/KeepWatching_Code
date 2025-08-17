using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CutCamListData", menuName = "Cutscene/CamData")]
public class CutCamListData : ScriptableObject
{
    public string SubTitle = "";
    public List<CameraWayPointNRot> WayList;
    public float CutTime = 0f;
    public bool IsEvent = false;
    public int EventCode = -1;
    public AudioClip Voice;
    public float DelayTime;
}

[System.Serializable]
public class CameraWayPointNRot
{
    public Vector3 Position;
    public Vector3 Rotation;
}