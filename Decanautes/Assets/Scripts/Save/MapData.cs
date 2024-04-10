using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : ScriptableObject
{
    [TitleGroup("DO NOT DELETE THIS FILE")]
    [HideInInspector]
    public string Name;
    [HideInInspector]
    public int CurrentCycle;
}
