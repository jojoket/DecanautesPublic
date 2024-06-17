using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerController playerController;

    public UIScreen CurrentUIScreen;

    public bool isBlocked;

    public FmodEventInfo EscapeFmod;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }



    public void Escape()
    {
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

}
