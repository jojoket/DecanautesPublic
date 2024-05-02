using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

public class EngineState : MonoBehaviour
{
    public enum EngineStateEnum
    {
        Good,
        Malfunction,
        BreakDown,
    }

    public EngineStateEnum CurrentState;

    public List<string> StateMessages;

    public TMP_Text EngineText;

    public Event LinkedEvent;
    public Interactable ResetButton;
    public Material ResetButtonMatBase;
    public Material ResetButtonMatWarning;

    public KilometerData KilometerData;

    [TitleGroup("Visual")]
    public List<GameObject> ToEnableOnGood = new List<GameObject>();
    public List<GameObject> ToDisableOnGood = new List<GameObject>();

    public List<GameObject> ToEnableOnMalfunction = new List<GameObject>();
    public List<GameObject> ToDisableOnMalfunction = new List<GameObject>();

    public List<GameObject> ToEnableOnBreakDown = new List<GameObject>();
    public List<GameObject> ToDisableOnBreakDown = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ResetButton.OnInteractStartedEvent.AddListener(FixBreakDown);
        ChangeState(EngineStateEnum.Good);
    }

    private void OnDestroy()
    {
        ResetButton.OnInteractStartedEvent.RemoveListener(FixBreakDown);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScreen();
    }

    public void ChangeState(EngineStateEnum state)
    {
        CurrentState = state;
        switch (CurrentState)
        {
            case EngineStateEnum.Good:
                GoodVfx();
                KilometerData.speedType = KilometerData.SpeedType.Normal;
                break;
            case EngineStateEnum.Malfunction:
                MalfunctionVfx();
                KilometerData.speedType = KilometerData.SpeedType.Malfunction;
                break;
            case EngineStateEnum.BreakDown:
                BreakDownVfx();
                KilometerData.speedType = KilometerData.SpeedType.Breakdown;
                break;
            default:
                break;
        }
    }

    public void UpdateScreen()
    {
        EngineText.text = "Engine state : " + CurrentState.ToString() + "\n";
        EngineText.text += " dist : " + KilometerData.CurrentKm + "km \n";
        EngineText.text += " speed : " + KilometerData.CurrentSpeed + "km/s \n";
        EngineText.text += StateMessages[(int)CurrentState] + "\n";
    }

    public void ChangeInteractions()
    {
        List<Interactable> temp = new List<Interactable>(LinkedEvent.InteractionsToFix);
        LinkedEvent.InteractionsToFix = new List<Interactable>(LinkedEvent.InteractionsToBreak);
        LinkedEvent.InteractionsToBreak = temp;
    }

    public void GoodVfx()
    {
        ResetButton.BaseMaterial = ResetButtonMatBase;
        ResetButton.ChangeMaterials(ResetButtonMatBase);
        foreach (GameObject gameObj in ToEnableOnGood)
        {
            gameObj.gameObject.SetActive(true);
        }
        foreach (GameObject gameObj in ToDisableOnGood)
        {
            gameObj.gameObject.SetActive(false);
        }
    }

    public void MalfunctionVfx()
    {
        ResetButton.BaseMaterial = ResetButtonMatBase;
        ResetButton.ChangeMaterials(ResetButtonMatBase);
        foreach (GameObject gameObj in ToEnableOnMalfunction)
        {
            gameObj.gameObject.SetActive(true);
        }
        foreach (GameObject gameObj in ToDisableOnMalfunction)
        {
            gameObj.gameObject.SetActive(false);
        }
    }

    public void BreakDownVfx()
    {
        ResetButton.BaseMaterial = ResetButtonMatWarning;
        ResetButton.ChangeMaterials(ResetButtonMatWarning);
        foreach (GameObject gameObj in ToEnableOnBreakDown)
        {
            gameObj.gameObject.SetActive(true);
        }
        foreach (GameObject gameObj in ToDisableOnBreakDown)
        {
            gameObj.gameObject.SetActive(false);
        }
    }

    public void FixBreakDown()
    {
        if (CurrentState != EngineStateEnum.BreakDown)
        {
            return;
        }
        ChangeState(EngineStateEnum.Good);
        ChangeInteractions();
    }
}
