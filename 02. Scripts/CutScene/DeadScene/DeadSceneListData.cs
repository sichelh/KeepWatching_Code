using UnityEngine;

[CreateAssetMenu(fileName = "DeadSceneListData", menuName = "Deadscene/SceneData")]
public class DeadSceneListData : ScriptableObject
{
    public string Name;
    public int Id;
    public string Descript;
    public Transform DeadPrefab;
    public Vector3 OffSet;
    public Vector3 LookOffset;
}