using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Event", menuName = "ScriptableObject/EventSystem/Event")]
public class Event : ScriptableObject
{

    public List<float> Probability;

    public List<GameObject> InteractionsToFix;
    public List<GameObject> ToEnableOnTrigger;
}
