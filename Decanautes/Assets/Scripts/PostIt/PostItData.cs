using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PostItData", menuName = "ScriptableObject/PostItData")]
public class PostItData : ScriptableObject
{
    public float DistanceMax;
    public Material ValidMaterial;
    public Material InvalidMaterial;
    public LayerMask PostItSurface;

}
