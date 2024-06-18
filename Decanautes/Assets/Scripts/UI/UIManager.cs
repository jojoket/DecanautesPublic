using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerController playerController;
    public GameObject HUD;

    public UIScreen CurrentUIScreen;

    public bool isBlocked;

    public FmodEventInfo EscapeFmod;
    public FmodEventInfo EscapeBlockingFmod;
    public FmodEventInfo OnChangeSceneFading;

    public Image FadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (isBlocked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        Instance = this;
        FadeIn();
    }



    public void Escape()
    {
        if (!isBlocked && CurrentUIScreen.ScreenEscape.IsBlocking)
            RythmManager.Instance.StartFmodEvent(EscapeBlockingFmod);
        else
            RythmManager.Instance.StartFmodEvent(EscapeFmod);
        CurrentUIScreen.gameObject.SetActive(false);
        if (CurrentUIScreen.ScreenEscape)
        {
            CurrentUIScreen.ScreenEscape.gameObject.SetActive(true);
            CurrentUIScreen = CurrentUIScreen.ScreenEscape;
            isBlocked = CurrentUIScreen.IsBlocking;
        }
        playerController.UIScreenBlock(isBlocked);
    }

    public void ChangeScene(int sceneIndex)
    {
        if (MapManager.Instance)
        {
            DOVirtual.DelayedCall(1, () =>
            {
                MapManager.Instance.SaveMap();
            });
        }
        Time.timeScale = 1;
        FadingScreen.DOFade(1, 2);
        if (RythmManager.Instance)
        {
            RythmManager.Instance.StartFmodEvent(OnChangeSceneFading);
        }
        DOVirtual.DelayedCall(3, () =>
        {
            SceneManager.LoadScene(sceneIndex);
        });
    }

    [Button]
    private void FadeIn()
    {
        FadingScreen.color = new Color(FadingScreen.color.r, FadingScreen.color.g, FadingScreen.color.b, 1);
        FadingScreen.DOFade(0, 2);
    }

    public void HideHUD(bool doHide)
    {
        HUD.SetActive(!doHide);
    }

}
