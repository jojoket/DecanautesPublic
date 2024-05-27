using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptedEventData", menuName = "ScriptableObject/ScriptedEventData")]
public class ScriptedEventData : ScriptableObject
{
    public float MaxTime;
    [MinMaxSlider(0, "MaxTime")]
    public Vector2 TaskAppearanceTimeInSec;
    [Unit(Units.Second)]
    public float FirstCycleStartTime;

    [TitleGroup("Debug")]
    [ReadOnly, Unit(Units.Second)]
    public float CurrentTimeLeft;
}
