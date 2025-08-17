using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : UIBase<UIPauseMenu>, IUIBase
{
    #region [Inspector]
    [Header("References")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private RenderTexture captureTexture;
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Material")]
    [SerializeField] private Material blurMaterial;
    
    #endregion


    #region [LifeCycle]
    protected override void Awake()
    {
        base.Awake();
    }

    #endregion


    #region [Menu]
    // 계속하기
    public void OnClickContinue()
    {
        InteractionSFX();

        UIManager.Instance.CheckOpenPopup(UIPauseMenu.Instance);
    }
    
    // 설정
    public void OnClickSettings()
    { 
        InteractionSFX();

        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    
    // 종료하기
    public void OnClickExit()
    {
        InteractionSFX();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    #endregion
    
    
    #region [UI]
    public override void Open()
    {
        base.Open();
        
        //퍼즈화면 캡쳐
        gameCamera.targetTexture = captureTexture;
        gameCamera.Render();
        gameCamera.targetTexture = null;
        //블러처리
        backgroundImage.texture = captureTexture;
        backgroundImage.material = blurMaterial;
        
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
        
        Time.timeScale = 0f;
    }

    public override void Close()
    {
        //설정창이 팝업되어있는 경우 메뉴로
        if (settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            base.Close();
        }
    }
    
    #endregion
    

    #region [Utils]
    private void InteractionSFX()
    {
        AudioManager.Instance.PlaySFX(SFX.UIHandle);
    }
    
    #endregion
}
