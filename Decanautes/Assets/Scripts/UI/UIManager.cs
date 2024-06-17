using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerController playerController;
    public GameObject HUD;

    public UIScreen CurrentUIScreen;

    public bool isBlocked;

    public FmodEventInfo EscapeFmod;
    public FmodEventInfo EscapeBlockingFmod;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
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
        SceneManager.LoadScene(sceneIndex);
    }

    public void HideHUD(bool doHide)
    {
        HUD.SetActive(!doHide);
    }

}
