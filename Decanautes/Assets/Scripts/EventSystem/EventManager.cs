using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System;
using System.Data;
using System.Runtime.CompilerServices;

using Decanautes.Interactable;


public class EventManager : MonoBehaviour
{
    //---------Params

    [TitleGroup("Parameters")]
    public float MaxSlider;
    [MinMaxSlider(0, "MaxSlider", ShowFields = true)]
    public Vector2 TaskAppearanceCycle;

    public List<Event> eventsToTrigger = new List<Event>();

    //---------Events
    [TitleGroup("Events")]
    public UnityEvent TaskTriggeredEvent;

    //---------Variables
    [TitleGroup("Debug")]
    [SerializeField, ReadOnly] private Event _currentEvent;
    [SerializeField, ReadOnly] private float _currentTimer;
    [SerializeField, ReadOnly] private float _currentEventTimer;
    [SerializeField, ReadOnly] private int _currentEventAmmount;
    [SerializeField, ReadOnly] private float _currentEventCoolDown;
    [SerializeField, ReadOnly] private int _numberOfEventsOccured;


    

    void Start()
    {
        _currentEventCoolDown = UnityEngine.Random.Range(TaskAppearanceCycle.x, TaskAppearanceCycle.y);
        foreach (Event childEvent in eventsToTrigger)
        {
            SetupEvent(childEvent);
        }
    }

    void Update()
    {
        ManageTimer();
        
        if (!_currentEvent && _currentEventAmmount >= 1)
        {
            _currentEventAmmount--;
            _currentEvent = StartNewEvent();
        }
        if (_currentEvent && CheckForCurrentTaskEnded(_currentEvent))
        {
            CleanLastEvent(_currentEvent);
            _currentEvent = null;
        }
    }

    private void OnDestroy()
    {
        foreach (Event childEvent in eventsToTrigger)
        {
            CleanOnDestroyEvent(childEvent);
        }
    }

    private void ManageTimer()
    {
        _currentTimer += Time.deltaTime;
        _currentEventTimer += Time.deltaTime;
        if (_currentEventTimer >= _currentEventCoolDown)
        {
            _currentEventTimer = 0;
            _currentEventCoolDown = UnityEngine.Random.Range(TaskAppearanceCycle.x, TaskAppearanceCycle.y);
            _currentEventAmmount += 1;
        }
    }

    private bool CheckForCurrentTaskEnded(Event parentEvent)
    {
        bool areChildsDone = true;
        foreach (Event childEvent in parentEvent.EventsToTrigger)
        {
            if (CheckForCurrentTaskEnded(childEvent) == false)
            {
                areChildsDone = false;
            }
        }
        if (areChildsDone)
        {
            if (parentEvent.InteractionsToFix.Count >= 1)
            {
                return !parentEvent.isActive;
            }
            else
            {
                parentEvent.isActive = false;
                return true;
            }
        }
        return false;
    }

    private void SetupEvent(Event parentEvent)
    {
        CheckForTaskFix(parentEvent);
        foreach (Event childEvent in parentEvent.EventsToTrigger)
        {
            SetupEvent(childEvent);
        }
    }

    //Will select next event and trigger it, from the EventManager Events
    private Event StartNewEvent()
    {
        List<float> eventsProba = new List<float>();
        foreach (Event childEvent in eventsToTrigger)
        {
            eventsProba.Add(childEvent.Probability*100);
        }
        int choosedEventIndex = Tools.WeightedRandom(eventsProba);
        Event choosedEvent = eventsToTrigger[choosedEventIndex];
        choosedEvent.isActive = true;
        choosedEvent.CurrentEvent = StartNewEvent(choosedEvent);
        EnableEventVFX(choosedEvent);
        TaskTriggeredEvent?.Invoke();
        return choosedEvent;
    }

    //Will select event and trigger it, from the given parent event
    private Event StartNewEvent(Event parentEvent)
    {
        if (parentEvent.EventsToTrigger.Count == 0)
        {
            return null;
        }
        List<float> eventsProba = new List<float>();
        foreach (Event childEvent in parentEvent.EventsToTrigger)
        {
            eventsProba.Add(childEvent.Probability * 100);
        }
        int choosedEventIndex = Tools.WeightedRandom(eventsProba);
        Event choosedEvent = parentEvent.EventsToTrigger[choosedEventIndex];
        choosedEvent.isActive = true;
        choosedEvent.CurrentEvent = StartNewEvent(choosedEvent);
        EnableEventVFX(choosedEvent);
        return choosedEvent;
    }

    //Will clean the last event and its children (vfx etc...)
    private void CleanLastEvent(Event lastEvent)
    {
        DisableEventVFX(lastEvent);
        lastEvent.isActive = false;
        lastEvent.InteractionsState.Clear();
        foreach (Interactable item in lastEvent.InteractionsToFix)
        {
            lastEvent.InteractionsState.Add(false);
        }
        foreach (Event childEvent in lastEvent.EventsToTrigger)
        {
            CleanLastEvent(childEvent);
        }
    }

    private void CleanOnDestroyEvent(Event lastEvent)
    {
        lastEvent.isActive = false;
        foreach (Interactable item in lastEvent.InteractionsToFix)
        {
            item.OnInteractStarted -= TaskInteracted;
        }

        foreach (Event item in lastEvent.EventsToTrigger)
        {
            CleanOnDestroyEvent(item);
        }
    }

    private void EnableEventVFX(Event eventTriggered)
    {
        foreach (GameObject obj in eventTriggered.ToEnableOnTrigger)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in eventTriggered.ToDisableOnTrigger)
        {
            obj.SetActive(false);
        }
    }

    private void DisableEventVFX(Event eventTriggered)
    {
        foreach (GameObject obj in eventTriggered.ToEnableOnTrigger)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in eventTriggered.ToDisableOnTrigger)
        {
            obj.SetActive(true);
        }
    }

    //-------FixTask

    private void CheckForTaskFix(Event eventToCheck)
    {
        eventToCheck.InteractionsState.Clear();
        foreach (Interactable item in eventToCheck.InteractionsToFix)
        {
            item.OnInteractStarted += TaskInteracted;
            item.LinkedEvent = eventToCheck;
            eventToCheck.InteractionsState.Add(false);
        }
    }

    private void TaskInteracted(Interactable interactable)
    {
        if (!interactable.LinkedEvent.isActive)
        {
            return;
        }
        int index = interactable.LinkedEvent.InteractionsToFix.FindIndex(x => x.gameObject == interactable.gameObject);
        if (index==-1)
        {
            return;
        }
        interactable.LinkedEvent.InteractionsState[index] = true;

        if (!interactable.LinkedEvent.InteractionsState.Contains(false))
        {
            CleanLastEvent(interactable.LinkedEvent);
        }
    }

}
