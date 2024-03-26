using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Event : MonoBehaviour
{
    public string Name;
    [HideInInspector]
    public bool isActive;
    [PropertyRange(0, 1)]
    public float Probability;

    
    [HideInInspector]
    public Event CurrentEvent;
    public List<Event> EventsToTrigger;

    public List<GameObject> ToEnableOnTrigger = new List<GameObject>();
    public List<GameObject> ToDisableOnTrigger = new List<GameObject>();

    public bool NeedToBeDoneInOrder = false;
    public List<Interactable> InteractionsToFix = new List<Interactable>();
    [HideInInspector]
    public List<bool> InteractionsState = new List<bool>();
}