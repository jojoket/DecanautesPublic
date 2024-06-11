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

    private int _currentInteractionFixIndex = -1;
    private List<Interactable> _interactionsToFix;

    // Start is called before the first frame update
    void Start()
    {
        GetInteractionsToFix(EventToTrigger);
        CheckForTaskFixBreak(EventToTrigger);
        if (MapManager.Instance.MapData.CurrentCycle == 1)
        {
            ScriptedEventData.CurrentTimeLeft = ScriptedEventData.FirstCycleStartTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EventToTrigger.isActive)
        {
            return;
        }
        ScriptedEventData.CurrentTimeLeft -= Time.deltaTime;

        if (ScriptedEventData.CurrentTimeLeft <= 0)
        {
            StartEvent();
        }

    }

    #region Cycle

    private void StartEvent()
    {
        EventToTrigger.isActive = true;
        EnableEventVFX(EventToTrigger, true);
        EventToTrigger.OnEnable?.Invoke();
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
        //don't do break if time max to fix is unlimited
        if (eventToFix.TimeMaxToFix != -1)
        {
            yield return new WaitForSeconds(eventToFix.TimeMaxToFix);
            if (eventToFix.isActive)
            {
                EventBreak(eventToFix);
            }
        }
    }

    public void EventBreak(Event eventBroken)
    {
        eventBroken.OnBreak?.Invoke();
        eventBroken.isActive = false;
        eventBroken.InteractionsState.Clear();
        foreach (Interactable item in _interactionsToFix)
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
        foreach (Interactable item in _interactionsToFix)
        {
            if (item.GetType() == typeof(InputScreen))
            {
                InputScreen inputScreen = (InputScreen)item;
                inputScreen.OnCodeValidAction += TaskInteracted;
            }
            else
            {
                item.OnInteractStarted += TaskInteracted;
            }
            item.LinkedEvent = eventToCheck;
            eventToCheck.InteractionsState.Add(false);
        }
        foreach (Interactable item in eventToCheck.InteractionsToBreak)
        {
            if (item.GetType() == typeof(InputScreen))
            {
                InputScreen inputScreen = (InputScreen)item;
                inputScreen.OnCodeValidAction += TaskInteracted;
            }
            else
            {
                item.OnInteractStarted += TaskInteracted;
            }
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
        
        int index = _interactionsToFix.FindIndex(x => x.gameObject == interactable.gameObject);
        if (index == -1)
        {
            return;
        }

        if (interactable.LinkedEvent.NeedToBeDoneInOrder && index != _currentInteractionFixIndex + 1)
        {
            //if interacted in wrong interaction order
            ResetButtonOrders(interactable);
            return;
        }

        interactable.LinkedEvent.InteractionsState[index] = true;
        _currentInteractionFixIndex = index;
        if (interactable.LinkedEvent.NeedToBeDoneInOrder)
        {
            interactable.ChangeMaterials(interactable.LinkedEvent.GoodOrderMaterial);
        }

        if (!interactable.LinkedEvent.InteractionsState.Contains(false))
        {
            CleanEvent(interactable.LinkedEvent);
        }
    }

    private void ResetButtonOrders(Interactable interactable)
    {
        _currentInteractionFixIndex = -1;
        for (int i = 0; i < interactable.LinkedEvent.InteractionsState.Count; i++)
        {
            interactable.LinkedEvent.InteractionsState[i] = false;
            _interactionsToFix[i].ChangeMaterials(interactable.BaseMaterial);
        }
    }

    private IEnumerator ResetButtonAfter(float sec, Interactable interactable)
    {
        yield return new WaitForSeconds(sec);
        ResetButtonOrders(interactable);
    }

    private void GetInteractionsToFix(Event LinkedEvent)
    {
        _interactionsToFix = LinkedEvent.InteractionsToFix;
        //if has secondary interaction (to change order for ex)
        if (LinkedEvent.HasSecondaryInteractions)
        {
            _interactionsToFix = LinkedEvent.CurrentInteractionFixSet == 0 ? LinkedEvent.InteractionsToFix : LinkedEvent.SecondaryInteractionsToFix;
        }
    }

    private void CleanEvent(Event eventToClean)
    {
        EnableEventVFX(eventToClean, false);
        eventToClean.isActive = false;
        eventToClean.InteractionsState.Clear();
        eventToClean.OnFix?.Invoke();
        if (eventToClean.HasSecondaryInteractions)
        {
            eventToClean.CurrentInteractionFixSet = Mathf.Abs(eventToClean.CurrentInteractionFixSet - 1);
        }
        foreach (Interactable item in _interactionsToFix)
        {
            eventToClean.InteractionsState.Add(false);
            StartCoroutine(ResetButtonAfter(1, item));
        }
        ScriptedEventData.CurrentTimeLeft = Random.Range(ScriptedEventData.TaskAppearanceTimeInSec.x, ScriptedEventData.TaskAppearanceTimeInSec.y);
        GetInteractionsToFix(eventToClean);
    }
    #endregion


    [Button]
    public void SwitchInteractions()
    {
        List<Interactable> temp = new List<Interactable>(EventToTrigger.InteractionsToFix);
        EventToTrigger.InteractionsToFix = new List<Interactable>(EventToTrigger.InteractionsToBreak);
        EventToTrigger.InteractionsToBreak = temp;
    }
}
