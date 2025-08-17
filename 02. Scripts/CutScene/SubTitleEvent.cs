using System.Collections;
using TMPro;
using UnityEngine;

public class SubTitleEvent : MonoBehaviour
{
    public TextMeshProUGUI SubtitleText;
    public float FadeTime;
    Coroutine FadeCorWrap;

    public void ChangeText(string value)
    {
        SubtitleText.text = value;
        if (FadeCorWrap != null)
        {
            StopCoroutine(FadeCorWrap);
        }
        FadeCorWrap = StartCoroutine(FadeSubTitle());
    }

    IEnumerator FadeSubTitle()
    {
        if (SubtitleText == null || string.IsNullOrEmpty(SubtitleText.text))
        {
            yield break;
        }

        float alpha = 0f;
        float timer = 0f;

        while (timer < FadeTime)
        {
            timer += Time.deltaTime;
            alpha = Mathf.Lerp(0f, 1f, timer / FadeTime);
            SubtitleText.GetComponent<CanvasGroup>().alpha = alpha;
            yield return null;
        }
    }
}
