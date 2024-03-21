using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Event", menuName = "ScriptableObject/Events/Event")]
public class Event : ScriptableObject
{
    public float Probability;

    public List<GameObject> InteractionsToFix;
    public List<GameObject> ToEnableOnTrigger;
    [TableList]
    public List<Event> EventsToTrigger;
}
