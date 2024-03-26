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
    [PropertyRange(0, 1)]
    public float Threshold = 1;
    public Color baseColor;
    public Color warningColor;
}