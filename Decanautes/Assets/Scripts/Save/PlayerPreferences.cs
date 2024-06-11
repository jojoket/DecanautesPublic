using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPreferences", menuName = "ScriptableObject/PlayerPreferences")]
public class PlayerPreferences : ScriptableObject
{
    //Controls
    public float CameraSensibility;
    public string Forward;
    public string Backward;
    public string Left;
    public string Right;
    public string Interact;
    public string Wright;

    //visuals
    public Color OutlineColor;
    public bool ContrastMode;
    public bool QuadrichromicMode;

    //sounds
    public float MusicVolume;
    public float AmbianceVolume;
    public float VFXVolume;
    public float UIVolume;
}
