using System.Collections;
using UnityEngine;

public class GameOpenSceneEvent : MonoBehaviour
{
    public CanvasGroup Hud;
    public CanvasGroup FadeOut;
    public GameObject OpenCutSceneCamera;
    public float FadeTime = 3f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Hud.alpha = 0;
        FadeOut.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        StartCoroutine(FadeOutCoroutine(FadeTime));
    }

    public void GetEvent(int @eventCode)
    {
        switch (@eventCode)
        {
            case 0:
            {
                break;
            }
            case 1:
            {
                break;
            }
            case 2:
            {
                OpenCutSceneCamera.SetActive(false);
                Hud.alpha = 1;
                mainCamera.gameObject.SetActive(true);
                break;
            }
        }
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        FadeOut.gameObject.SetActive(true);
        FadeOut.blocksRaycasts = true;
        FadeOut.interactable = true;

        float startAlpha = FadeOut.alpha;
        float timer      = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            FadeOut.alpha = Mathf.Lerp(startAlpha, 0f, progress);

            yield return null;
        }

        FadeOut.alpha = 0f;
        FadeOut.blocksRaycasts = false;
        FadeOut.interactable = false;
    }
}