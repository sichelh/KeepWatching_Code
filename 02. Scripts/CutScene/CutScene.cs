using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    public Camera MainCamera;
    public List<CutCamListData> CutCamLists;
    public SubTitleEvent SubtitleText;
    public string NextSceneName;

    [Header("Cut Animation Event")]
    public UnityEvent<int> unityEvent;

    void Start()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;
        }
        MainCamera.gameObject.SetActive(true);
        SubtitleText.GetComponent<TextMeshProUGUI>().text = "";
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        AudioManager.Instance.PlayBGM(BGM.StartSceneMusic);
        foreach (CutCamListData cutCam in CutCamLists)
        {
            SubtitleText.ChangeText(cutCam.SubTitle); // 자막 바꿈

            if (cutCam.Voice != null)
            {
                AudioManager.Instance.PlaySFXDirect(cutCam.Voice); // 음성 실행
            }
            MainCamera.transform.position = cutCam.WayList[0].Position; // 카메라 위치 초기화

            float timer = 0f;
            float segmentTime = cutCam.CutTime / (cutCam.WayList.Count - 1);

            for (int i = 0; i < cutCam.WayList.Count - 1; i++) // 카메라 이동
            {
                Vector3 startPos = cutCam.WayList[i].Position;
                Vector3 endPos = cutCam.WayList[i + 1].Position;
                Quaternion startRot = Quaternion.Euler(cutCam.WayList[i].Rotation);
                Quaternion endRot = Quaternion.Euler(cutCam.WayList[i + 1].Rotation);

                timer = 0f;
                while (timer < segmentTime)
                {
                    timer += Time.deltaTime;
                    float t = Mathf.Clamp01(timer / segmentTime);
                    MainCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
                    MainCamera.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
                    yield return null;
                }
            }

            if (cutCam.IsEvent == true)
            {
                unityEvent?.Invoke(cutCam.EventCode);
            }

            yield return new WaitForSeconds(cutCam.DelayTime);
        }
        SubtitleText.ChangeText("");
        GotoGameScene();
    }

    void GotoGameScene()
    {
        // 게임 씬으로
        // NextSceneName
        if (NextSceneName != "" && NextSceneName != null)
        {
            SceneManager.LoadScene(NextSceneName);
        }
        // MainCamera.gameObject.SetActive(false);
    }

}
