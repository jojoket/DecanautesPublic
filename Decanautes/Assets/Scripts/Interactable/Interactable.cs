using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


public class AnimatorTriggerer
{
    public Animator animator;
    public enum ParameterType
    {
        Trigger,
        Bool,
        Float,
    }
    public ParameterType triggerType;
    public string parameterName;

}

public class Interactable : MonoBehaviour
{
    //--------Components
    public List<Renderer> renderers;
    public Material BaseMaterial;
    public Material HoverMaterial;
    public Material ActiveMaterial;

    //--------Events
    public UnityAction<Interactable> OnInteractStarted;
    public UnityEvent OnInteractStartedEvent;
    public UnityAction<Interactable> OnInteractEnded;
    public UnityEvent OnInteractEndedEvent;
    [HideInInspector]
    public Event LinkedEvent;
    [HideInInspector]
    public Maintain LinkedMaintainable;
    [ReadOnly]
    public bool isActivated;
    [ReadOnly]
    public bool isPressed;



    void Start()
    {
    }

    void Update()
    {
        
    }

    public virtual void Hover()
    {
        //Activate Outline
        foreach (Renderer renderer in renderers)
        {
            renderer.material = HoverMaterial;
        }

    }

    public virtual void StopHover()
    {
        //Deactivate Outline
        foreach (Renderer renderer in renderers)
        {
            renderer.material = BaseMaterial;
        }
    }

    public virtual void InteractionStart()
    {
        OnInteractStarted?.Invoke(this);
        OnInteractStartedEvent?.Invoke();
        isPressed = true;
    }

    public virtual void InteractionEnd()
    {
        OnInteractEnded?.Invoke(this);
        OnInteractEndedEvent?.Invoke();
        isPressed = false;
    }
}
