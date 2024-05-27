using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Decanautes.Interactable;

public class Event : MonoBehaviour
{
    [TabGroup("General")]
    public string Name;
    [HideInInspector]
    public bool isActive;
    [PropertyRange(0, 1)]
    [HideInInspector]
    public float Probability;
    [TabGroup("General"), Tooltip("Set to -1 to have unlimited time")]
    public float TimeMaxToFix = -1;


    [HideInInspector]
    public Event CurrentEvent;
    [HideInInspector]
    public List<Event> EventsToTrigger;

    [TabGroup("Visual")]
    public List<GameObject> ToEnableOnTrigger = new List<GameObject>();
    [TabGroup("Visual")]
    public List<GameObject> ToDisableOnTrigger = new List<GameObject>();
    [TabGroup("Visual")]
    public Material GoodOrderMaterial;
    [ReadOnly]
    public EngineState EngineLinked;
    [TabGroup("Events")]
    public UnityEvent OnEnable;
    [TabGroup("Events")]
    public UnityEvent OnFix;
    [TabGroup("Events")]
    public UnityEvent OnBreak;

    [TabGroup("Interactions")]
    public bool NeedToBeDoneInOrder = false;
    [TabGroup("Interactions")]
    public List<Interactable> InteractionsToFix = new List<Interactable>();
    [TabGroup("Interactions")]
    public List<Interactable> InteractionsToBreak = new List<Interactable>();
    [HideInInspector]
    public List<bool> InteractionsState = new List<bool>();
}