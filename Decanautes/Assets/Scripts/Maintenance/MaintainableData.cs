using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "Maintainable", menuName = "ScriptableObject/Maintainable")]
public class MaintainableData : ScriptableObject
{
    [TitleGroup("Parameters")]
    public string Name;
    [PropertyRange(0, 1)]
    public float CurrentState = 1;
    [PropertyRange(0, 1)]
    public float FirstCycleState = 1;
    public float Speed = -1;
    public bool NeedThresholdToMaintain = true;
    [PropertyRange(0, 1)]
    public float Threshold = 1;
    [TitleGroup("Visual")]
    [Unit(Units.Second)]
    public float FillingTime;
    public Material BaseMaterial;
    public Material WarningMaterial;
}