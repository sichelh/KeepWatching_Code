using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewindEffecter : MonoBehaviour
{
    public static RewindEffecter Instance { get; private set; }

    [SerializeField] private Image effectImage;

    [Header("페이드 시간")]
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float alphaMax = 20f / 255f; // 최대 알파값

    // 현재 실행중인 페이드 코루틴
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        Instance = this;

        if (effectImage != null)
        {
            var color = effectImage.color;
            color.a = 0f;
            effectImage.color = color;
            effectImage.gameObject.SetActive(false);
        }
    }

    public void ShowEffect()
    {
        if (effectImage != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            effectImage.gameObject.SetActive(true);
            fadeCoroutine = StartCoroutine(Fade(alphaMax));
        }
    }

    public void CloseEffect()
    {
        if (effectImage != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            // 알파값을 0으로 줄이면서, 페이드 후에 비활성화 콜백 실행
            fadeCoroutine = StartCoroutine(Fade(0f, () => { effectImage.gameObject.SetActive(false); }));
        }
    }

    // 보간 코루틴
    // OnComplete는 페이드 종료 후 실행할 콜백 함수
    private IEnumerator Fade(float targetAlpha, System.Action onComplete = null)
    {
        float startAlpha = effectImage.color.a;
        float time       = 0f;

        while (time < fadeDuration)
        {
            // 타임스케일을 다른 데서 쓰고 있어서, 영향을 받지 않게끔 unscaledDeltaTime을 씀
            time += Time.unscaledDeltaTime;

            // 보간
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);

            var color = effectImage.color;
            color.a = alpha;
            effectImage.color = color;

            yield return null;
        }

        // 반복문 종료 후 정확히 세팅
        var finalColor = effectImage.color;
        finalColor.a = targetAlpha;
        effectImage.color = finalColor;

        // onComplete가 null이 아니면 SetActive(false) 실행
        onComplete?.Invoke();

        // 코루틴 종료
        fadeCoroutine = null;
    }
}