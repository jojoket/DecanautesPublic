using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class ObjectSave
{
    public string Name;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public string PrefabPath;

    //For interactables objects
    public bool IsActivated;

    //For Post it objects
    public string PostItText;
    public int CycleNum = -1;

    //For Events objects
    public List<string> EventInteractionsTofix = new List<string>();
    public List<string> EventInteractionsToBreak = new List<string>();
}

public class SavedObject : MonoBehaviour
{
    public string originalPrefabPath;
}
