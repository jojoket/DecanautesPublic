using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

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

    // Start is called before the first frame update
    void Start()
    {
        ChangeStateToGood();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(EngineStateEnum state)
    {
        CurrentState = state;
        EngineText.text = "Engine state : " + state.ToString();
        EngineText.text += StateMessages[(int)state];
        if (CurrentState == EngineStateEnum.Good)
        {
            List<Interactable> temp = new List<Interactable>(LinkedEvent.InteractionsToFix);
            LinkedEvent.InteractionsToFix = new List<Interactable>(LinkedEvent.InteractionsToBreak);
            LinkedEvent.InteractionsToBreak = temp;
        }
    }

    public void ChangeStateToMalfunction()
    {
        CurrentState = EngineStateEnum.Malfunction;
        EngineText.text = "Engine state : " + EngineStateEnum.Malfunction.ToString();
        EngineText.text += StateMessages[(int)EngineStateEnum.Malfunction];
        if (CurrentState == EngineStateEnum.Good)
        {
            List<Interactable> temp = new List<Interactable>(LinkedEvent.InteractionsToFix);
            LinkedEvent.InteractionsToFix = new List<Interactable>(LinkedEvent.InteractionsToBreak);
            LinkedEvent.InteractionsToBreak = temp;
        }
    }

    public void ChangeStateToBreakDown()
    {
        CurrentState = EngineStateEnum.BreakDown;
        EngineText.text = "Engine state : " + EngineStateEnum.BreakDown.ToString();
        EngineText.text += StateMessages[(int)EngineStateEnum.BreakDown];
        if (CurrentState == EngineStateEnum.Good)
        {
            List<Interactable> temp = new List<Interactable>(LinkedEvent.InteractionsToFix);
            LinkedEvent.InteractionsToFix = new List<Interactable>(LinkedEvent.InteractionsToBreak);
            LinkedEvent.InteractionsToBreak = temp;
        }
    }

    public void ChangeStateToGood()
    {
        CurrentState = EngineStateEnum.Good;
        EngineText.text = "Engine state : " + EngineStateEnum.Good.ToString();
        EngineText.text += StateMessages[(int)EngineStateEnum.Good];
        if (CurrentState == EngineStateEnum.Good)
        {
            List<Interactable> temp = new List<Interactable>(LinkedEvent.InteractionsToFix);
            LinkedEvent.InteractionsToFix = new List<Interactable>(LinkedEvent.InteractionsToBreak);
            LinkedEvent.InteractionsToBreak = temp;
        }
    }
}
