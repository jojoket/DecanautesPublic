
# **Le Flux des Decanautes**

You incarnate a **Decanaute**, an entity that only exists between short time spans, some says they're infinite.  
Here you are, incarnated in a **room seemingly mechanic**. The clicks of the machines make out a melodious song. You only have one thought : **Maintain it.**
## Features

- Machine puzzles
- Day to night cycle
- Post Its
- Permanency (Save and load system)
- Tools (Game Events, Animation triggers, Sound triggers, Logic triggers)
- Sound/Music/SFX/Animation synchronization
- Custom Shaders
- Multiple Lighting layouts

## Screenshots

### Concept Arts

![App Screenshot](https://lh3.googleusercontent.com/d/1RrjkC5RIDFAjR29rFEXEpo1WcMj43khA)
![App Screenshot](https://lh3.googleusercontent.com/d/1bLNrDQsT0SGrTPK2LkfjNa6NQkMHm7LF)
![App Screenshot](https://lh3.googleusercontent.com/d/1OSSSkDznrfr9ZqiKRHIm-MfWQ5O4Sr3-)


## Programmation Tools and systems

I tried my best in this project to ease the interaction between the designers/artists and the game systems. 

> I tried to do this by making simple "tools".  
> Like the `MaterialAnimation.cs` class or `PropAnimation.cs`.

In this section I will go through the most used and interesting ones.

### Event System

The game is made up of "events" (not the code ones), each machine's malfunction is an event.  

#### Event Manager
The design of the events changed during the production of the game, so there is one system I made we ended up not using.  
It launched a weighed random event from a pool of events, each one with a weight. Each event could be an action (Machine malfunction) or a container of other events to trigger that would be chosen again by a weighed random.

```c#
//Will select next event and trigger it, from the EventManager Events
private Event StartNewEvent()
{
  //take the probabilities
  List<float> eventsProba = new List<float>();
  foreach (Event childEvent in eventsToTrigger)
  {
      eventsProba.Add(childEvent.Probability*100);
  }
  //get random event
  int choosedEventIndex = Tools.WeightedRandom(eventsProba);
  Event choosedEvent = eventsToTrigger[choosedEventIndex];
  //activate it
  choosedEvent.isActive = true;
  choosedEvent.CurrentEvent = StartNewEvent(choosedEvent);
  EnableEventVFX(choosedEvent);
  TaskTriggeredEvent?.Invoke();
  return choosedEvent;
}
```

#### Scripted Event

We ended up going for a more straightforward approach. Each event have a timer before it is triggered. When an event is triggered it waits to be fixed to restart a timer again.

```c#
private void StartEvent()
{
  foreach (FmodEventInfo fmodEvent in EventToTrigger.OnEnableFmod)
  {
    RythmManager.Instance.StartFmodEvent(fmodEvent);
  }
  EventToTrigger.isActive = true;
  EnableEventVFX(EventToTrigger, true);
  EventToTrigger.OnEnable?.Invoke();
  StartCoroutine(TimeLimit(EventToTrigger));
  TaskTriggeredEvent?.Invoke();
}
```

We check if an event's fixed by using `UnityAction` and checking when one of the event's Interactables is used, if it's the good one it fixs the event.

```c#
private void CheckForTaskFixBreak(Event eventToCheck)
{
  eventToCheck.InteractionsState.Clear();
  foreach (Interactable item in _interactionsToFix)
  {
    if (item.GetType() == typeof(InputScreen))
    {
      InputScreen inputScreen = (InputScreen)item;
      inputScreen.CodeNeeded = MapManager.Instance.MapData.CurrentCycle.ToString();
      inputScreen.OnCodeValidAction += TaskInteracted;
    }
    else
    {
      item.OnInteractStarted += TaskInteracted;
    }
    item.LinkedEvent = eventToCheck;
    eventToCheck.InteractionsState.Add(false);
  }
  foreach (Interactable item in eventToCheck.InteractionsToBreak)
  {
    if (item.GetType() == typeof(InputScreen))
    {
      InputScreen inputScreen = (InputScreen)item;
      inputScreen.CodeNeeded = MapManager.Instance.MapData.CurrentCycle.ToString();
      inputScreen.OnCodeValidAction += TaskInteracted;
    }
    else
    {
      item.OnInteractStarted += TaskInteracted;
    }
    item.LinkedEvent = eventToCheck;
  }
}
```

### Interactables

### Animation Triggers
### Sound Triggers and Rythm Manager
### Save system and Map Manager

## Authors

#### Graphics
- [@Hugo Bayle]()
- [@Morgane Bru]()
#### Game Design
- [@Théodore Laborde](https://theodorelbrd.wixstudio.io/portfolio)
#### Management
- [@Timothée Bolla](https://boolti.itch.io/)
#### UX Ergonomics
- [@Lud.e Chatin]()
#### Programming
- [@Jules Sarton du Jonchay](https://github.com/jojoket)
#### Sound and Music
- [@Mael Heurard](https://lazytuned.itch.io/)

