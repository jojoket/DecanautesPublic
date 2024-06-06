using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public UIScreen ScreenEscape;
    public bool IsBlocking;

    public void Activate(bool active)
    {
        gameObject.SetActive(active);
        if (active)
        {
            UIManager.Instance.CurrentUIScreen = this;
        }
    }
}
