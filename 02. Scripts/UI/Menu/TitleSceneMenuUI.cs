using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneMenuUI : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClickStart()
    {
        InteractionSFX();

        SceneManager.LoadScene("StartScene");
    }

    public void OnClickExit()
    {
        InteractionSFX();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    private void InteractionSFX()
    {
        AudioManager.Instance.PlaySFX(SFX.UIHandle);
    }
}