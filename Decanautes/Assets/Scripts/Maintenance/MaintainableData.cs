using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Maintainable", menuName = "ScriptableObject/Maintainable")]
public class MaintainableData : ScriptableObject
{
    public string Name;
    [PropertyRange(0, 1)]
    public float CurrentState = 1;
    public float Speed = -1;
    public bool NeedThresholdToMaintain = true;
    [PropertyRange(0, 1)]
    public float Threshold = 1;
    [TabGroup("Visual")]
    public float FillingTime;
    public Color baseColor;
    public Color warningColor;
}