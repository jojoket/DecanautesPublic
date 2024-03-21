using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "EventManagerData", menuName = "ScriptableObject/Events/EventManagerData")]
public class EventManagerData : ScriptableObject
{
    [MinMaxSlider(0,20,ShowFields = true)]
    public Vector2 TaskAppearanceCycle;

    [TableList]
    public List<Event> eventsToTrigger = new List<Event>();


}
