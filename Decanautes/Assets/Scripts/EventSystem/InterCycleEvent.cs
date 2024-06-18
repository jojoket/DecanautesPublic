using Decanautes.Interactable;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct InterEventInCycleEvent
{
    [Unit(Units.Minute)]
    public float TriggerTime;
    public UnityEvent OnTrigger;
    public List<FmodEventInfo> FmodEvents;
}

public class InterCycleEvent : MonoBehaviour
{
    [TabGroup("Components")]
    public InterCycleEventData data;
    [TabGroup("Components")]
    public EngineState LinkedEngineState;

    [TabGroup("Events")]
    public List<InterEventInCycleEvent> EventsToTrigger;
    [TabGroup("Events")]
    public List<InterEventInCycleEvent> EventsToTriggerOnBreak;

    [TabGroup("Interactions")]
    public List<Interactable> ToPreventBreakDown;




    // Start is called before the first frame update
    void Start()
    {
        if (MapManager.Instance.MapData.CurrentCycle == 1)
        {
            data.CyclesBeforeEvent = data.FirstGapCycleNum;
        }
        data.CyclesBeforeEvent--;

        //start interCycles Messages

        foreach (InterCycleMessage message in data.interCycleMessages)
        {
            if (data.CyclesBeforeEvent == message.CyclesBefore)
            {
                StartCoroutine(WaitBeforeMessage(message));
            }
        }


        if (data.CyclesBeforeEvent > 0 )
        {
            return;
        }
        data.CyclesBeforeEvent = UnityEngine.Random.Range((int)data.CycleRandomGap.x, (int)data.CycleRandomGap.y);
        //StartEvents during inter cycle event
        foreach (InterEventInCycleEvent CycleEvent in EventsToTrigger)
        {
            StartCoroutine(WaitBeforeEvent(CycleEvent));
        }
        foreach (InterEventInCycleEvent CycleEvent in EventsToTriggerOnBreak)
        {
            StartCoroutine(WaitBeforeEventBreak(CycleEvent));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WaitBeforeMessage(InterCycleMessage message)
    {
        yield return new WaitForSeconds(message.MessageTimeInCycle * 60);
        LinkedEngineState.DisplayMessage(message);
        foreach (FmodEventInfo FmodEvent in message.FmodEvents)
        {
            FmodEvent.EventPosition = transform;
            RythmManager.Instance.StartFmodEvent(FmodEvent);
        }
    }

    private IEnumerator WaitBeforeEvent(InterEventInCycleEvent EventWaiting)
    {
        yield return new WaitForSeconds(EventWaiting.TriggerTime * 60);

        EventWaiting.OnTrigger?.Invoke();
        foreach (FmodEventInfo FmodEvent in EventWaiting.FmodEvents)
        {
            RythmManager.Instance.StartFmodEvent(FmodEvent);
        }
    }

    private IEnumerator WaitBeforeEventBreak(InterEventInCycleEvent EventWaiting)
    {
        yield return new WaitForSeconds(EventWaiting.TriggerTime * 60);
        bool isBroken = false;
        foreach (Interactable interactable in ToPreventBreakDown)
        {
            if (!interactable.isActivated)
            {
                isBroken = true;
            }
        }
        if (isBroken)
        {
            EventWaiting.OnTrigger?.Invoke();
            foreach (FmodEventInfo FmodEvent in EventWaiting.FmodEvents)
            {
                RythmManager.Instance.StartFmodEvent(FmodEvent);
            }
        }
    }

}
