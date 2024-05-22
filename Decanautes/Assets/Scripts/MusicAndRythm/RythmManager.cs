using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using Sirenix.OdinInspector;
using System;

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


    //From FMOD

    public TimelineInfo timelineInfo = null;
    public EventReference Music;
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

        if (!Music.IsNull)
        {
            _musicInstance = RuntimeManager.CreateInstance(Music);
            _musicInstance.start();
        }


    }

    private void Start()
    {
        if(!Music.IsNull)
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
            TriggerBeatEvent();
        }
    }

    
    private void TriggerBeatEvent()
    {
        OnBeatTrigger?.Invoke();
        if (isFirst)
        {
            isFirst = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/TestEvent/TestSync");
        }

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
