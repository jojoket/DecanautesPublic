using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class script_Button_Transition : MonoBehaviour
{
    // Start is called before the first frame update


    public CinemachineVirtualCamera Camera1;

    public UnityEngine.UI.Button StartButton;

    void Start()
    {
        //Button btn = StartButton.GetComponent<Button>();
        StartButton.onClick.AddListener(TaskOnClick);
    }

    private void OnDestroy()
    {
        StartButton.onClick.RemoveListener(TaskOnClick);
    }

    // Update is called once per frame
    void TaskOnClick()
    {
        Camera1.gameObject.SetActive(false);
    }
}
