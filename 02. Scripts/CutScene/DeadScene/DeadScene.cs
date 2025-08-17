using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DeadScene : MonoBehaviour
{
    [Header("Test")]
    public bool IsTest = false;

    public Transform DeadCam;
    public Transform Enemy;
    public List<DeadSceneListData> DeadScenes;
    public int SceneNum;
    private Transform deadPrefab;

    public event Action OnDeadScene;

    void Start()
    {
        DeadCam.gameObject.SetActive(false);
        if (IsTest == true)
        {
            ShowDeadCam(); // 테스트
        }

        GameManager.Instance.OnGameOver += ShowDeadCam;
    }

    public void ShowDeadCam()
    {
        UIHUD.Instance.gameObject.SetActive(false);
        PostProcessVolume postProcessVolume = DeadCam.GetComponent<PostProcessVolume>();
        postProcessVolume.weight = 0f;

        SceneNum = UnityEngine.Random.Range(0, DeadScenes.Count);
        DeadCam.gameObject.SetActive(true);
        // DeadCam.transform.position = Enemy.position + DeadScenes[SceneNum].OffSet;

        // 데드씬 프리팹 인스턴티에이트 후, 방향 카메라를 보게.
        GameObject @go = Instantiate(DeadScenes[SceneNum].DeadPrefab, DeadScenes[SceneNum].DeadPrefab.transform.position, Quaternion.identity).gameObject;
        deadPrefab = @go.transform;
        PlayerController.Instance.transform.position = go.transform.position;
        DeadCam.transform.position = deadPrefab.position + DeadScenes[SceneNum].OffSet;
        Vector3    directionToTarget = DeadCam.position - deadPrefab.position;
        Quaternion targetRotation    = Quaternion.LookRotation(directionToTarget, Vector3.up);
        deadPrefab.rotation = Quaternion.Euler(deadPrefab.rotation.x, targetRotation.y, deadPrefab.rotation.z);
        OnDeadScene?.Invoke();
        Enemy.gameObject.SetActive(false);

        // 데드씬 카메라 위치, 방향 설정
        DeadCam.LookAt(deadPrefab.position + DeadScenes[SceneNum].LookOffset);
        deadPrefab.GetComponent<CutEventAble>().PlayEvent();
        AudioManager.Instance.PlaySFX(SFX.FearGhost);
    }

    public void OnCameraEffect()
    {
        PostProcessVolume postProcessVolume = DeadCam.GetComponent<PostProcessVolume>();
        postProcessVolume.weight = 1f;
    }
}