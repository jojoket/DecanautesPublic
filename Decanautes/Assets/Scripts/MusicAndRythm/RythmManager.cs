using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;

[Serializable]
public class FmodEventInfo
{
    public Transform EventPosition;
    public EventReference FmodReference;
}


public class RythmManager : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    //Singleton
    public static RythmManager Instance;

    public UnityEvent OnBeatTrigger;

    private bool isFirst = true;
    public int lastBeat = 0;

    private List<FmodEventInfo> FMODEvents = new List<FmodEventInfo>();


    //From FMOD

    public TimelineInfo timelineInfo = null;
    public FmodEventInfo Base;
    public List<FmodEventInfo> EventsOnStart = new List<FmodEventInfo>();
    private GCHandle _timelineHandle;
    private FMOD.Studio.EventInstance _musicInstance;
    private FMOD.Studio.EVENT_CALLBACK _beatCallBack;



    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Rythm Manager already exist");
            return;
        }
        Instance = this;

        if (!Base.FmodReference.IsNull)
        {
            _musicInstance = RuntimeManager.CreateInstance(Base.FmodReference);
            _musicInstance.start();
        }


    }

    private void Start()
    {
        if(!Base.FmodReference.IsNull)
        {
            timelineInfo = new TimelineInfo();
            _beatCallBack = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
            _timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
            _musicInstance.setUserData(GCHandle.ToIntPtr(_timelineHandle));
            _musicInstance.setCallback(_beatCallBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        }
    }

    void Update()
    {
        if (timelineInfo.currentBeat != lastBeat)
        {
            lastBeat = timelineInfo.currentBeat;
            TriggerBeatEvent();
        }
    }

    
    private void TriggerBeatEvent()
    {
        OnBeatTrigger?.Invoke();
        PlayAndRelieveBuffer();
        if (isFirst)
        {
            foreach (FmodEventInfo fmodEvent in EventsOnStart)
            {
                AddFModEventToBuffer(fmodEvent);
            }
            isFirst = false;
        }
    }

    public void AddFModEventToBuffer(FmodEventInfo fmodEventAdded)
    {
        FMODEvents.Add(fmodEventAdded);
    }

    private void PlayAndRelieveBuffer()
    {
        foreach (FmodEventInfo fmodEvent in FMODEvents)
        {
            FMODUnity.RuntimeManager.PlayOneShot(fmodEvent.FmodReference, fmodEvent.EventPosition.position);
        }
        FMODEvents.Clear();
    }



    private void OnDestroy()
    {
        OnBeatTrigger.RemoveAllListeners();

        _musicInstance.setUserData(IntPtr.Zero);
        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _musicInstance.release();
        _timelineHandle.Free();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.Box($"Current Beat = {timelineInfo.currentBeat}, last marker = {(string)timelineInfo.lastMarker}");
    }
#endif

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        if( result != FMOD.RESULT.OK )
        {
            Debug.LogError("TimeLine CallBack error " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero )
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;
            switch(type )
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBeat = parameter.beat;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                        break;
                    }
            }
        }
        return FMOD.RESULT.OK;
    }
}
