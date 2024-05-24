using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

using Decanautes.Interactable;

public class ScriptedEvent : MonoBehaviour
{
    public ScriptedEventData ScriptedEventData;

    public Event EventToTrigger;

    [TitleGroup("Events")]
    public UnityEvent TaskTriggeredEvent;

    // Start is called before the first frame update
    void Start()
    {
        CheckForTaskFixBreak(EventToTrigger);
        if (MapManager.Instance.MapData.CurrentCycle == 0)
        {
            ScriptedEventData.CurrentTimeLeft = ScriptedEventData.FirstCycleStartTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ScriptedEventData.CurrentTimeLeft -= Time.deltaTime;

        if (ScriptedEventData.CurrentTimeLeft < 0)
        {
            ScriptedEventData.CurrentTimeLeft = Random.Range(ScriptedEventData.TaskAppearanceTimeInSec.x, ScriptedEventData.TaskAppearanceTimeInSec.y);
            StartEvent();
        }

    }

    #region Cycle

    private void StartEvent()
    {
        EventToTrigger.isActive = true;
        EnableEventVFX(EventToTrigger, true);
        EventToTrigger.OnEnable?.Invoke();
        if (EventToTrigger.EngineLinked)
        {
            EventToTrigger.EngineLinked.ChangeState(EngineState.EngineStateEnum.Malfunction);
        }
        StartCoroutine(TimeLimit(EventToTrigger));
        TaskTriggeredEvent?.Invoke();
    }

    //Make all the event's object active etc
    private void EnableEventVFX(Event eventTriggered, bool state)
    {
        foreach (GameObject obj in eventTriggered.ToEnableOnTrigger)
        {
            obj.SetActive(state);
        }
        foreach (GameObject obj in eventTriggered.ToDisableOnTrigger)
        {
            obj.SetActive(!state);
        }
    }

    private IEnumerator TimeLimit(Event eventToFix)
    {
        yield return new WaitForSeconds(eventToFix.TimeMaxToFix);
        if (eventToFix.isActive)
        {
            EventBreak(eventToFix);
        }
    }
    public void EventBreak(Event eventBroken)
    {
        eventBroken.OnBreak?.Invoke();
        if (eventBroken.EngineLinked)
        {
            eventBroken.EngineLinked.ChangeState(EngineState.EngineStateEnum.BreakDown);
        }
        eventBroken.isActive = false;
        eventBroken.InteractionsState.Clear();
        foreach (Interactable item in eventBroken.InteractionsToFix)
        {
            eventBroken.InteractionsState.Add(false);
        }
    }

    #endregion

    #region Event interaction

    //subscribe the events to check the task's end
    private void CheckForTaskFixBreak(Event eventToCheck)
    {
        eventToCheck.InteractionsState.Clear();
        foreach (Interactable item in eventToCheck.InteractionsToFix)
        {
            item.OnInteractStarted += TaskInteracted;
            item.LinkedEvent = eventToCheck;
            eventToCheck.InteractionsState.Add(false);
        }
        foreach (Interactable item in eventToCheck.InteractionsToBreak)
        {
            item.OnInteractStarted += TaskInteracted;
            item.LinkedEvent = eventToCheck;
        }
    }

    private void TaskInteracted(Interactable interactable)
    {
        if (!interactable.LinkedEvent.isActive)
        {
            return;
        }
        int indexBreak = interactable.LinkedEvent.InteractionsToBreak.FindIndex(x => x.gameObject == interactable.gameObject);
        if (indexBreak != -1)
        {
            EventBreak(interactable.LinkedEvent);
            return;
        }
        int index = interactable.LinkedEvent.InteractionsToFix.FindIndex(x => x.gameObject == interactable.gameObject);
        if (index == -1)
        {
            return;
        }
        interactable.LinkedEvent.InteractionsState[index] = true;

        if (!interactable.LinkedEvent.InteractionsState.Contains(false))
        {
            CleanEvent(interactable.LinkedEvent);
        }
    }
    private void CleanEvent(Event eventToClean)
    {
        EnableEventVFX(eventToClean, false);
        eventToClean.isActive = false;
        eventToClean.InteractionsState.Clear();
        eventToClean.OnFix?.Invoke();
        foreach (Interactable item in eventToClean.InteractionsToFix)
        {
            eventToClean.InteractionsState.Add(false);
        }
        if (eventToClean.EngineLinked)
        {
            eventToClean.EngineLinked.ChangeState(EngineState.EngineStateEnum.Good);
            eventToClean.EngineLinked.ChangeInteractions();
        }
    }
    #endregion

}
