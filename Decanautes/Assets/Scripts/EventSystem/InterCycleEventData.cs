using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InterCycleMessage
{
    [Tooltip("How many cycles before")]
    public int CyclesBefore;
    public string Message;
    [Unit(Units.Minute)]
    public float MessageTimeInCycle;
    [Unit(Units.Second)]
    public float MessageDuration;
    public Color MessageColor;
    public Color MessageBackgroundColor;
    public List<FmodEventInfo> FmodEvents;
}

[CreateAssetMenu(fileName = "InterCycleEventData", menuName = "ScriptableObject/InterCycleEventData")]
public class InterCycleEventData : ScriptableObject
{
    [TitleGroup("Cycle Params")]
    public int CyclesBeforeEvent;
    public int FirstGapCycleNum;
    public Vector2 CycleRandomGap;
    public List<InterCycleMessage> interCycleMessages;
}
