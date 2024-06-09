using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using Decanautes.Interactable;
using System;
using FMODUnityResonance;

[Serializable]
public struct EngineLinkedMachineText
{
    public TMP_Text Text;
    public Event linkedEvent;
    public MaintainableData linkedMaintainable;
}

public class EngineState : MonoBehaviour
{
    public enum EngineStateEnum
    {
        Good,
        Malfunction,
        BreakDown,
    }

    [TabGroup("Components")]
    public List<Event> LinkedEvents = new List<Event>();
    [TabGroup("Components")]
    public List<MaintainableData> LinkedMaintainables = new List<MaintainableData>();
    [TabGroup("Components")]
    public Interactable ResetButton;
    [TabGroup("Components")]
    public Material ResetButtonMatBase;
    [TabGroup("Components")]
    public Material ResetButtonMatWarning;
    [TabGroup("Components")]
    public KilometerData KilometerData;


    [TabGroup("Parameters"), SerializeField]
    private float MalfunctionsInfluence;

    [TabGroup("Visual")]
    public List<GameObject> ToEnableOnGood = new List<GameObject>();
    [TabGroup("Visual")]
    public List<GameObject> ToDisableOnGood = new List<GameObject>();

    [TabGroup("Visual")]
    public List<GameObject> ToEnableOnMalfunction = new List<GameObject>();
    [TabGroup("Visual")]
    public List<GameObject> ToDisableOnMalfunction = new List<GameObject>();

    [TabGroup("Visual")]
    public List<GameObject> ToEnableOnBreakDown = new List<GameObject>();
    [TabGroup("Visual")]
    public List<GameObject> ToDisableOnBreakDown = new List<GameObject>();

    [TabGroup("FMOD")]
    public List<FmodEventInfo> OnStateGoodFmodEvents;
    [TabGroup("FMOD")]
    public List<FmodEventInfo> OnStateMalfunctionFmodEvents;
    [TabGroup("FMOD")]
    public List<FmodEventInfo> OnPowerDecreaseFmodEvent;
    [TabGroup("FMOD")]
    public List<FmodEventInfo> OnPowerIncreaseFmodEvent;
    [TabGroup("FMOD")]
    public List<FmodEventInfo> OnStateBreakdownFmodEvents;

    [TabGroup("Texts")]
    public List<string> StateMessages;
    [TabGroup("Texts")]
    public TMP_Text GeneralText;
    [TabGroup("Texts")]
    public TMP_Text PowerLevelText;
    [TabGroup("Texts")]
    public TMP_Text StateText;
    [TabGroup("Texts")]
    public TMP_Text StateMessageText;
    [TabGroup("Texts")]
    public TMP_Text DistanceText;
    [TabGroup("Texts")]
    public TMP_Text SpeedText;
    [TabGroup("Texts")]
    public List<EngineLinkedMachineText> LinkedMachinesTexts = new List<EngineLinkedMachineText>();
    

    [TabGroup("Debug"), ReadOnly]
    public EngineStateEnum CurrentState;
    [TabGroup("Debug"), SerializeField, ReadOnly]
    public int malfunctionsNumber;

    // Start is called before the first frame update
    void Start()
    {
        SetupLinkedEventsAndMaintainables();
        ChangeState(EngineStateEnum.Good);
        UpdateMachinesText();
    }

    private void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScreens();
    }

    private void SetupLinkedEventsAndMaintainables()
    {
        ResetButton.OnInteractStartedEvent.AddListener(FixBreakDown);
        foreach (Event linkedEvent in LinkedEvents)
        {
            linkedEvent.EngineLinked = this;
            linkedEvent.OnBreak.AddListener(GoToBreakDown);
            linkedEvent.OnEnable.AddListener(CheckForLinkedEventsAndMaintainables);
            linkedEvent.OnFix.AddListener(CheckForLinkedEventsAndMaintainables);
        }
    }

    #region State logic

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
                KilometerData.CurrentSpeed = KilometerData.NormalSpeed * (1- MalfunctionsInfluence*malfunctionsNumber);
                break;
            case EngineStateEnum.BreakDown:
                BreakDownVfx();
                KilometerData.speedType = KilometerData.SpeedType.Breakdown;
                break;
            default:
                break;
        }
    }

    public void CheckForLinkedEventsAndMaintainables()
    {
        int lastMalfunctionNum = malfunctionsNumber;
        malfunctionsNumber = 0;
        foreach(Event linkedEvent in LinkedEvents)
        {
            if (linkedEvent.isActive)
            {
                malfunctionsNumber++;
            }
        }
        foreach(MaintainableData maintainable in LinkedMaintainables)
        {
            if (maintainable.CurrentState <= maintainable.Threshold)
            {
                malfunctionsNumber++;
            }
        }

        //sound
        if (malfunctionsNumber < lastMalfunctionNum)
        {
            foreach (FmodEventInfo fmodEvent in OnPowerDecreaseFmodEvent)
            {
                RythmManager.Instance.StartFmodEvent(fmodEvent);
            }
        }
        if (malfunctionsNumber > lastMalfunctionNum)
        {
            foreach (FmodEventInfo fmodEvent in OnPowerIncreaseFmodEvent)
            {
                RythmManager.Instance.StartFmodEvent(fmodEvent);
            }
        }

        UpdateMachinesText();
        if (malfunctionsNumber == 0)
        {
            ChangeState(EngineStateEnum.Good);
            return;
        }
        if (malfunctionsNumber <= LinkedEvents.Count + LinkedMaintainables.Count)
        {
            ChangeState(EngineStateEnum.Malfunction);
            return;
        }
        //ChangeState(EngineStateEnum.BreakDown);
    }


    #endregion

    #region Visual

    public void UpdateScreens()
    {
        if (GeneralText)
        {
            GeneralText.text = "Engine state : " + CurrentState.ToString() + "\n";
            GeneralText.text += " Power : " + (1 - MalfunctionsInfluence * malfunctionsNumber)*100 + "% \n";
            GeneralText.text += " dist : " + KilometerData.CurrentKm + "km \n";
            GeneralText.text += " speed : " + KilometerData.CurrentSpeed + "km/s \n";
            GeneralText.text += StateMessages[(int)CurrentState] + "\n";
        }

        if (PowerLevelText)
        {
            PowerLevelText.text = "Power : " + (1 - MalfunctionsInfluence * malfunctionsNumber) * 100 + "% \n";
        }
        if (StateText)
        {
            StateText.text = "Engine state : " + CurrentState.ToString();
        }
        if (StateMessageText)
        {
            StateMessageText.text = StateMessages[(int)CurrentState];
        }
        if (DistanceText)
        {
            DistanceText.text = "dist : " + KilometerData.CurrentKm + "km";
        }
        if (SpeedText)
        {
            SpeedText.text = "speed : " + KilometerData.CurrentSpeed + "km/s";
        }
    }
    public void UpdateMachinesText()
    {
        //set every machines texts
        for (int i = 0; i < LinkedMachinesTexts.Count; i++)
        {
            TMP_Text textMP = LinkedMachinesTexts[i].Text;
            string textToDisplay = "";
            if (LinkedMachinesTexts[i].linkedEvent != null)
            {
                textToDisplay += LinkedMachinesTexts[i].linkedEvent.Name + "'s state : " + (LinkedMachinesTexts[i].linkedEvent.isActive ? "Broken" : "OK");
            }
            else
            {
                textToDisplay += LinkedMachinesTexts[i].linkedMaintainable.Name + "'s state : " + (LinkedMachinesTexts[i].linkedMaintainable.CurrentState <= LinkedMachinesTexts[i].linkedMaintainable.Threshold ? "Broken" : "OK");

            }

            textMP.text = textToDisplay;
        }
    }

    public void GoodVfx()
    {
        foreach (FmodEventInfo fmodEvent in OnStateGoodFmodEvents)
        {
            RythmManager.Instance.StartFmodEvent(fmodEvent);
        }
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
        foreach (FmodEventInfo fmodEvent in OnStateMalfunctionFmodEvents)
        {
            RythmManager.Instance.StartFmodEvent(fmodEvent);
        }
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
        foreach (FmodEventInfo fmodEvent in OnStateBreakdownFmodEvents)
        {
            RythmManager.Instance.StartFmodEvent(fmodEvent);
        }
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
    #endregion

    [Button]
    public void FixBreakDown()
    {
        if (CurrentState != EngineStateEnum.BreakDown)
        {
            return;
        }
        ChangeState(EngineStateEnum.Good);
    }

    [Button]
    public void GoToBreakDown()
    {
        if (CurrentState == EngineStateEnum.BreakDown)
        {
            return;
        }
        ChangeState(EngineStateEnum.BreakDown);
    }
}
