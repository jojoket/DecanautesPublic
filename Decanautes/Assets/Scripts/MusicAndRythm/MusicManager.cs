using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public enum MusicState
    {
        Silent,
        AllGood,
        Creshendo,
        Broken,
        Repaired,
        Ending,
    }

    public float TimeUntilNextEvent;
    public float MinTimeForMusicStart;
    public float MinGameTimeForMusicStart;
    public float CreshendoTime;
    public float EndingTime;
    private float MusicStartTimeStamp;
    public FmodEventInfo MusicFmodEvent;
    public EngineState CurrentEngineState;

    public MusicState CurrentMusicState;

    [ParamRef]
    public string MusicChangeState;

    // Start is called before the first frame update
    void Start()
    {
        CurrentMusicState = MusicState.Silent;
    }

    // Update is called once per frame
    void Update()
    {
        TimeUntilNextEvent = GetTimeUntilNextEvent();

        if (CurrentMusicState == MusicState.Silent && TimeUntilNextEvent > MinTimeForMusicStart && Time.time >= MinGameTimeForMusicStart)
        {
            MusicStartTimeStamp = Time.time;
            if (MusicFmodEvent.EventPosition)
                RythmManager.Instance.StartFmodEvent(MusicFmodEvent);
            CurrentMusicState = MusicState.Creshendo;
            StartCoroutine(ChangeToAllGood());
        }
        if (CurrentMusicState == MusicState.AllGood && TimeUntilNextEvent <= 0)
        {
            CurrentMusicState = MusicState.Broken;
            RythmManager.Instance.ChangeFmodParameter(MusicChangeState, 1);
            //Commencer Boucle Broken
        }
        if (CurrentMusicState == MusicState.Broken && CurrentEngineState.malfunctionsNumber <= 0)
        {
            CurrentMusicState = MusicState.Repaired;
            RythmManager.Instance.ChangeFmodParameter(MusicChangeState, 2);
            //Commencer Boucle Repaired
        }
        if (CurrentMusicState == MusicState.Repaired && CurrentEngineState.malfunctionsNumber > 0)
        {
            CurrentMusicState = MusicState.Ending;
            RythmManager.Instance.ChangeFmodParameter(MusicChangeState, 3);
            //Commencer fin de musique
            StartCoroutine(ChangeToSilent());
        }
    }

    private IEnumerator ChangeToAllGood()
    {
        yield return new WaitForSeconds(CreshendoTime);
        CurrentMusicState = MusicState.AllGood;
    }

    private IEnumerator ChangeToSilent()
    {
        yield return new WaitForSeconds(EndingTime);
        CurrentMusicState = MusicState.Silent;
    }

    private float GetTimeUntilNextEvent()
    {
        float time = 10000;
        ScriptedEvent[] scriptedEvents = GameObject.FindObjectsByType<ScriptedEvent>(FindObjectsSortMode.None);

        foreach (ScriptedEvent scriptedEvent in scriptedEvents)
        {
            if (scriptedEvent.ScriptedEventData.CurrentTimeLeft <= time)
            {
                time = scriptedEvent.ScriptedEventData.CurrentTimeLeft;
            }
        }

        CareSystem[] careSystems = GameObject.FindObjectsByType<CareSystem>(FindObjectsSortMode.None);

        foreach (CareSystem careSystem in careSystems)
        {
            foreach(Maintain maintain in careSystem.Maintainables)
            {
                float comparedTime = maintain.MaintainableData.CurrentState / Mathf.Abs(maintain.MaintainableData.Speed);
                if (comparedTime <= time)
                {
                    time = comparedTime;
                }
            }
        }
        return time;
    }
}
