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
            RythmManager.Instance.StartFmodEvent(MusicFmodEvent);
            CurrentMusicState = MusicState.Creshendo;
            StartCoroutine(ChangeToAllGood());
        }
        if (CurrentMusicState == MusicState.AllGood && TimeUntilNextEvent <= 0)
        {
            CurrentMusicState = MusicState.Broken;
            //Commencer Boucle Broken
        }
        if (CurrentMusicState == MusicState.Broken && CurrentEngineState.malfunctionsNumber <= 0)
        {
            CurrentMusicState = MusicState.Repaired;
            //Commencer Boucle Repaired
        }
        if (CurrentMusicState == MusicState.Repaired && CurrentEngineState.malfunctionsNumber > 0)
        {
            CurrentMusicState = MusicState.Ending;
            //Commencer fin de musique
            StartCoroutine(ChangeToSilent());
        }
    }

    private IEnumerator ChangeToAllGood()
    {
        yield return new WaitForSecondsRealtime(CreshendoTime);
        CurrentMusicState = MusicState.AllGood;
    }

    private IEnumerator ChangeToSilent()
    {
        yield return new WaitForSecondsRealtime(EndingTime);
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