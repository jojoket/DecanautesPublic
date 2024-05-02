using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //For Events objects
    public List<Interactable> EventInteractionsTofix;
    public List<Interactable> EventInteractionsToBreak;
}

public class SavedObject : MonoBehaviour
{
    public string originalPrefabPath;
}
