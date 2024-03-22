using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Event", menuName = "ScriptableObject/Events/Event")]
public class Event : ScriptableObject
{
    public string Name;
    [PropertyRange(0,1)]
    public float Probability;

    [HideInTables]
    public bool NeedToBeDoneInOrder = false;
    [HideInTables]
    public List<Interactable> InteractionsToFix = new List<Interactable>();
    [HideInInspector, HideInTables]
    public List<bool> InteractionsState = new List<bool>();
    [HideInTables]
    public List<GameObject> ToEnableOnTrigger = new List<GameObject>();
    [HideInTables]
    public List<GameObject> ToDisableOnTrigger = new List<GameObject>();

    [TableList, HideInTables]
    public List<Event> EventsToTrigger;
    [HideInInspector]
    public Event CurrentEvent;
}
