using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class LastMinute : MonoBehaviour
{
    public static LastMinute Instance { get; private set; }
    [Unit(Units.Minute)]
    public float LastMinuteTime;
    [Unit(Units.Second)]
    public float LastMinuteDuration;
    [Unit(Units.Second)]
    public float LastDecaNoteTime;
    public EngineState EngineState;

    [ReadOnly]
    public bool IsLastMinute;
    [ReadOnly]
    public bool IsLastDecaNote = false;
    public InterCycleMessage LastMinuteMessage;
    public UnityEvent OnLastMinute;
    public UnityEvent OnGameEnding;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance)
        {
            Debug.LogWarning("LastMinute Already Exists");
            return;
        }
        Instance = this;
        IsLastMinute = false;
        IsLastDecaNote = false;
        StartCoroutine(WaitForLastMinute());
    }

    private void Update()
    {
        if (IsLastMinute)
        {
            float secLeft = Mathf.Max(LastMinuteDuration - (Time.time - (LastMinuteTime*60)), 0);
            LastMinuteMessage.Message = "Your cycle ends...\n" + Mathf.Floor(secLeft / 60) + ":" +  Math.Floor(secLeft % 60);
            if (secLeft <= LastDecaNoteTime)
            {
                IsLastDecaNote = true;
            }
        }
    }

    public void EndGame()
    {
        IsLastDecaNote = false;
        IsLastMinute = false;
        OnGameEnding?.Invoke();
    }

    public IEnumerator WaitForLastMinute()
    {
        yield return new WaitForSeconds(LastMinuteTime*60);
        IsLastMinute = true;
        foreach (FmodEventInfo fmodEvent in LastMinuteMessage.FmodEvents)
        {
            RythmManager.Instance.StartFmodEvent(fmodEvent);
        }
        EngineState.DisplayMessage(LastMinuteMessage);
        OnLastMinute?.Invoke();
    }

}
