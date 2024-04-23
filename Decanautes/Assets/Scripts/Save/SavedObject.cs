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
    public string PostItText;
    public string PrefabPath;
    public bool IsActivated;
}

public class SavedObject : MonoBehaviour
{
    public string originalPrefabPath;
}
