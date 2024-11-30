
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

Concerning [interactables](https://github.com/jojoket/DecanautesPublic/tree/MainNew1/Decanautes/Assets/Scripts/Interactable), as there is multiple possible interactable :
* Button
* Grabbable
* InputScreen
* Lever
* (and there could've been more)

I decided to make a [parent class](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/Interactable/Interactable.cs) that each type of interactable would inherit.

Here is the base methods of each interactables :  

![Capture d’écran 2024-11-30 172757](https://github.com/user-attachments/assets/c18d8722-8544-43bf-a290-27cb6ba8049e)

Another handy thing about interactables is that their logic can be setup through the editor quite simply with [Unity Events](https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html) :  

![Capture d’écran 2024-11-30 173431](https://github.com/user-attachments/assets/4b292699-17f1-45b1-83ca-c84dd35436ee)


### Animation Triggers

The [animation triggers](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/Props/AnimatorAndSoundTrigger.cs) is a simple structure of data made to handle simple manipulation of animator parameters for designers/Graphists.  

For example on the end of an interaction here :  

![Capture d’écran 2024-11-30 182658](https://github.com/user-attachments/assets/ac91d06d-3c59-4a18-b0a3-593aa2c9680e)

The graphist can change an animator bool parameter to true without having to code it.  

It's made so that I just had to make an array of animation triggers whenever something happened and called each of them in code, and the designers could add and remove them however they wanted.  
It permitted a lot of **simplicity** and **freedom** for graphists **to integrate** their animations.

I also made a Monobehavior "[Prop Animation](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/Props/PropAnimation.cs)" that is a standalone version of it (so that graphists can put animation triggers on something else than interactables), it can be triggered with an editor button or through **Unity Events**.

![Capture d’écran 2024-11-30 175828](https://github.com/user-attachments/assets/56387791-e251-471a-ade9-ecf8a14266f5)


#### Material animations

Alternatively I also made a [version of it](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/Props/MaterialChangement.cs) for the **materials**. It takes a material parameter and lerp it from a value to another in a set time, juste like the other it can be triggered from Unity Events or an editor button.

![Capture d’écran 2024-11-30 180325](https://github.com/user-attachments/assets/1e8776b2-7508-4b73-8481-d2194ed9c94d)


### Sound Triggers and Rythm Manager

The game has a huge sound and musical interest. One of our main objectives was to make certain animations and sound effects synchronized with the music.  
To do that, I used FMOD to track the current rythm with a [rythm manager singleton](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/MusicAndRythm/RythmManager.cs). Every script can add an fmod sound event to a buffer in the manager. Manager that will, in turn, clean the buffer and play the sounds when the next beat occurs.

```c#
  private void PlayAndRelieveBuffer()
    {
        List<FmodEventInfo> fmodEventToDelete = new List<FmodEventInfo>();
        foreach (FmodEventInfo fmodEvent in FMODEvents)
        {
            if (fmodEvent.isBeatSpecific && fmodEvent.BeatStart != timelineInfo.currentBeat)
            {
                continue;
            }
            FMODUnity.RuntimeManager.PlayOneShot(fmodEvent.FmodReference, fmodEvent.EventPosition.position);
            fmodEventToDelete.Add(fmodEvent);
        }
        foreach (KeyValuePair<string,int>  FmodParameter in FMODParameters)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(FmodParameter.Key, FmodParameter.Value);
        }
        foreach (FmodEventInfo fmodEvent in fmodEventToDelete)
        {
            FMODEvents.Remove(fmodEvent);
        }
        FMODParameters.Clear();
    }
```

### Save system and Map Manager

The [save system](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/Save/SaveManager.cs) is a pretty simple system that will keep every scriptable objects given to it saved as jsons and load them when the game starts.

The [map manager](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/Save/MapManager.cs) will get all gameobjects with a SavedObject component and save it in a [mapData](https://github.com/jojoket/DecanautesPublic/blob/MainNew1/Decanautes/Assets/Scripts/Save/MapData.cs) scriptable object that is saved by the save system.

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

