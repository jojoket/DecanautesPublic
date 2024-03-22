using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System;
using System.Data;
using System.Runtime.CompilerServices;

public class EventManager : MonoBehaviour
{
    //---------Params

    [TitleGroup("Parameters")]
    public EventManagerData EventManagerData;

    //---------Events
    [TitleGroup("Events")]
    public UnityEvent TaskTriggeredEvent;

    //---------Variables
    [TitleGroup("Debug")]
    [SerializeField, ReadOnly] private Event _currentEvent;
    [SerializeField, ReadOnly] private float _currentTimer;
    [SerializeField, ReadOnly] private float _currentTaskTimer;
    [SerializeField, ReadOnly] private int _currentTaskAmmount;
    [SerializeField, ReadOnly] private float _currentTaskCoolDown;
    [SerializeField, ReadOnly] private int _numberOfEventsOccured;


    

    void Start()
    {
        _currentTaskCoolDown = UnityEngine.Random.Range(EventManagerData.TaskAppearanceCycle.x, EventManagerData.TaskAppearanceCycle.y);
    }

    void Update()
    {
        CheckForTaskFix();
        ManageTimer();
        if (!_currentEvent && _currentTaskAmmount >= 1)
        {
            _currentTaskAmmount--;
            _currentEvent = StartNewEvent();
        }
    }

    private void ManageTimer()
    {
        _currentTimer += Time.deltaTime;
        _currentTaskTimer += Time.deltaTime;
        if (_currentTaskTimer >= _currentTaskCoolDown)
        {
            _currentTaskTimer = 0;
            _currentTaskCoolDown = UnityEngine.Random.Range(EventManagerData.TaskAppearanceCycle.x, EventManagerData.TaskAppearanceCycle.y);
            _currentTaskAmmount += 1;
        }
    }

    //Will select next event and trigger it, from the EventManager Events
    private Event StartNewEvent()
    {
        List<float> eventsProba = new List<float>();
        foreach (Event childEvent in EventManagerData.eventsToTrigger)
        {
            eventsProba.Add(childEvent.Probability*100);
        }
        int choosedEventIndex = Tools.WeightedRandom(eventsProba);
        Event choosedEvent = EventManagerData.eventsToTrigger[choosedEventIndex];
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
        choosedEvent.CurrentEvent = StartNewEvent(choosedEvent);
        EnableEventVFX(choosedEvent);
        return choosedEvent;
    }

    //Will clean the last event and its children (vfx etc...)
    private void CleanLastEvent(Event lastEvent)
    {
        DisableEventVFX(lastEvent);
        foreach (Interactable item in lastEvent.InteractionsToFix)
        {
            item.OnInteractStarted -= TaskInteracted;
        }
        foreach (Event childEvent in lastEvent.EventsToTrigger)
        {
            CleanLastEvent(childEvent);
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
            eventToCheck.InteractionsState.Add(false);
        }

        CheckForTaskFix(eventToCheck.CurrentEvent);
    }

    private void TaskInteracted()
    {
        bool isFixed = false;
        if (isFixed)
        {
            CleanLastEvent(_currentEvent);
            _currentEvent = null;
        }
    }



}
