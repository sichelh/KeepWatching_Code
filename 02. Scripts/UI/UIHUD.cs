using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class UIHUD : SceneOnlySingleton<UIHUD>
{
    [SerializeField] List<QuickSlot> quickSlots;


    private InputHandler inputHandler;

    [Header("Heart Beat")]
    [SerializeField] [Range(50, 200)] private int heartRate;

    [SerializeField] private float minVolume = 0.05f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float lowScale = 1.05f;
    [SerializeField] private float highScale = 1.2f;
    [SerializeField] private TextMeshProUGUI heartRateTxt;
    [SerializeField] private Transform heartTransform;
    [SerializeField] private Image ecgImage;
    private Coroutine heartbeatCoroutine;
    private Material ecgMat;

    [Header("Stamina")]
    [SerializeField] private Image staminaGuageImage;

    [SerializeField] private float curValue;
    [SerializeField] private float maxValue;

    [Header("Tubular Vision")]
    [SerializeField] private Image vignetteOverlay;

    Color vignetteColor;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 0.8f;

    [Header("CamaraViewer")]
    [SerializeField] GameObject camaraViewer;


    public List<QuickSlot> QuickSlots => quickSlots;

    protected override void Awake()
    {
        base.Awake();
        ecgMat = Instantiate(ecgImage.material);
        ecgImage.material = ecgMat;
        vignetteColor = vignetteOverlay.color;
    }

    private void Start()
    {
        inputHandler = GameManager.Instance.InputHandler;
        inputHandler.OnQuickSlotPressed += UseItemAtSlot;

        // PlayerController.Instance.PlayerStat.playerStat[StatType.CurrentStamina].OnValueChanged += (curValue)
        //     =>
        // {
        //     UpdateStaminaUI(curValue, PlayerController.Instance.PlayerStat.GetValue(StatType.MaxStamina));
        // };
        PlayerController.Instance.PlayerStat.playerStat[StatType.CurrentStamina].OnValueChanged += UpdateStaminaUIWrapper;
    }

    private void UseItemAtSlot(int index)
    {
        if (quickSlots[index].RegistedItem == null)
            return;
        InventoryManager.Instance.UseItem(quickSlots[index].RegistedInventoryIndex);
    }

    private void OnEnable()
    {
        StartHeartbeat();
    }

    private void OnDisable()
    {
        StopHeartbeat();
    }

    void Update()
    {
        heartRateTxt.text = heartRate.ToString();
        UpdateVignette();
    }
    //===============================================================
    // 시스템 관련 UI


    //===============================================================
    // 연출 관련 UI

    #region [HeartBeat]

    private void StartHeartbeat()
    {
        if (heartbeatCoroutine != null)
            StopCoroutine(heartbeatCoroutine);
        heartbeatCoroutine = StartCoroutine(HeartbeatRoutine());
    }

    private void StopHeartbeat()
    {
        if (heartbeatCoroutine != null)
        {
            StopCoroutine(heartbeatCoroutine);
            heartbeatCoroutine = null;
        }
    }

    private IEnumerator HeartbeatRoutine()
    {
        while (true)
        {
            float interval = 60f / heartRate;
            float volume   = LerpByHeartRate(maxVolume, minVolume);

            AudioManager.Instance.SetHeartVolume(volume);
            AudioManager.Instance.Playheart(HEART.Heartbeat);

            AnimateHeart(interval);
            AnimateECG(interval);
            AnimateVignette(interval);

            yield return new WaitForSeconds(interval);
        }
    }

    private void AnimateHeart(float interval)
    {
        heartTransform.DOKill();
        heartTransform.localScale = Vector3.one;

        float   scaleFactor = LerpByHeartRate(highScale, lowScale);
        Vector3 enlarged    = Vector3.one * scaleFactor;
        Vector3 shrunk      = Vector3.one / scaleFactor;

        DOTween.Sequence()
            .Append(heartTransform.DOScale(enlarged, interval * 0.25f).SetEase(Ease.OutQuad))
            .Append(heartTransform.DOScale(shrunk, interval * 0.2f).SetEase(Ease.InOutQuad))
            .Append(heartTransform.DOScale(Vector3.one, interval * 0.15f).SetEase(Ease.InQuad));
    }

    public void SetHeartRate(int newRate)
    {
        heartRate = Mathf.Clamp(newRate, 50, 200);
    }

    #endregion

    #region [ECG]

    private Transform interactionRayPointTransform;

    private void AnimateECG(float interval)
    {
        ecgMat.SetFloat("_Cutoff", 0f);
        DOTween.To(() => ecgMat.GetFloat("_Cutoff"),
                x => ecgMat.SetFloat("_Cutoff", x),
                1.2f, interval)
            .SetEase(Ease.Linear);
    }

    #endregion

    #region [Tubular Vision]

    void UpdateVignette()
    {
        float t     = Mathf.Clamp01((heartRate - 80f) / 120f);
        float alpha = LerpByHeartRate(maxAlpha, minAlpha);

        vignetteColor.a = alpha;
        vignetteOverlay.color = vignetteColor;
    }

    private void AnimateVignette(float interval)
    {
        Transform vignetteTransform = vignetteOverlay.transform;
        vignetteTransform.DOKill();
        vignetteTransform.localScale = Vector3.one;

        float   scaleFactor = 1.05f;
        Vector3 enlarged    = Vector3.one * scaleFactor;

        DOTween.Sequence()
            .Append(vignetteTransform.DOScale(enlarged, interval * 0.25f).SetEase(Ease.OutQuad))
            .Append(vignetteTransform.DOScale(Vector3.one, interval * 0.4f).SetEase(Ease.InOutQuad));
    }

    #endregion

    #region [Stamina]

    private void UpdateStaminaUI(float current, float max)
    {
        staminaGuageImage.fillAmount = current / max;
    }

    private void UpdateStaminaUIWrapper(float cur)
    {
        UpdateStaminaUI(cur, PlayerController.Instance.PlayerStat.GetValue(StatType.MaxStamina));
    }

    #endregion

    #region [CamaraViewer]

    public void CamaraViewer(bool isOn)
    {
        camaraViewer.SetActive(isOn);
    }

    #endregion

    //===============================================================

    #region [Util]

    private float LerpByHeartRate(float max, float min)
    {
        float lerpValue = (max - min) * ((heartRate - 50f) / 150f) + min;
        return lerpValue;
    }

    #endregion

    protected override void OnDestroy()
    {
        base.OnDestroy();
        inputHandler.OnQuickSlotPressed -= UseItemAtSlot;
        PlayerController.Instance.PlayerStat.playerStat[StatType.CurrentStamina].OnValueChanged -= UpdateStaminaUIWrapper;
    }
}