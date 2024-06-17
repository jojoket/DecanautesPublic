using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/PlayerData", order =0)]
public class PlayerData : ScriptableObject
{
    [TitleGroup("Parameters", Alignment = TitleAlignments.Centered)]
    [Title("Movement")]
    public float MoveSpeed;
    [MinValue(1f)]
    public float Drag;
    public float InteractionMaxDist;
    [MinMaxSlider(0,2)]
    public Vector2 FootStepsCycle;

    
}
