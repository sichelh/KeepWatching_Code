using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtoneImageRenderer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region [Inspector]
    public Image bloodImage;

    #endregion
    

    #region [LifeCycle]
    void OnEnable()
    {
        if (bloodImage != null)
        {
            DOTween.Kill(bloodImage);
            bloodImage.fillAmount = 0f;
        }
    }
    
    #endregion


    #region [ImageRenderer]
    // 마우스오버 시 이미지 렌더
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(SFX.UIHandle);

        if (bloodImage != null)
        {
            DOTween.Kill(bloodImage);
            bloodImage.DOFillAmount(1f, 0.1f).SetUpdate(true);
        }
    }

    // 마우스아웃 시 비가시화
    public void OnPointerExit(PointerEventData eventData)
    {
        if (bloodImage != null)
        {
            DOTween.Kill(bloodImage);
            bloodImage.fillAmount = 0f;
        }
    }

    #endregion
    
}
