using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObject/MapData")]
public class MapData : ScriptableObject
{
    [TitleGroup("DO NOT DELETE THIS FILE")]
    public string Name;
    public int CurrentCycle;
    public List<ObjectSave> SavedObjects = new List<ObjectSave>();
}
