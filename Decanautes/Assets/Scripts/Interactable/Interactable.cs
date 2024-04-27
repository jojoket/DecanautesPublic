using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

[Serializable]
public class AnimatorTriggerer
{
    public Animator animator;
    public enum ParameterType
    {
        Trigger,
        Bool,
        Float,
        Vector3,
    }
    public ParameterType triggerType;
    public string parameterName;
    [ShowIf("triggerType", ParameterType.Bool)]
    public bool StateToApply;
    [ShowIf("triggerType", ParameterType.Float)]
    public float FloatToApply;
    [ShowIf("triggerType", ParameterType.Vector3)]
    public Vector3 Vector3ToApply;

    public void TriggerAnimation()
    {
        switch (triggerType)
        {
            case ParameterType.Trigger:
            {
                animator.SetTrigger(parameterName);
                break;
            }
            case ParameterType.Bool:
            {
                animator.SetBool(parameterName, StateToApply);
                break;
            }
            case ParameterType.Float:
            {
                animator.SetFloat(parameterName, FloatToApply);
                break;
            }
            case ParameterType.Vector3:
            {
                break;
            }
        }
    }

}

public class Interactable : MonoBehaviour
{
    //--------Components
    [TitleGroup("Components")]
    public List<Renderer> renderers;
    public Material BaseMaterial;
    public Material HoverMaterial;
    public Material ActiveMaterial;

    [TitleGroup("Parameters")]
    public bool IsToggle = false;
    public bool NeedLookToKeepInteraction = true;


    //--------Events
    public List<AnimatorTriggerer> OnInteractStartedAnimations = new List<AnimatorTriggerer>();
    public UnityAction<Interactable> OnInteractStarted;
    public UnityEvent OnInteractStartedEvent;
    public List<AnimatorTriggerer> OnInteractEndedAnimations = new List<AnimatorTriggerer>();
    public UnityAction<Interactable> OnInteractEnded;
    public UnityEvent OnInteractEndedEvent;
    [HideInInspector]
    public Event LinkedEvent;
    [HideInInspector]
    public Maintain LinkedMaintainable;
    [TitleGroup("Debug")]
    public bool isActivated = false;
    [ReadOnly]
    public bool isPressed;



    void Start()
    {
        if (!IsToggle)
            return;
        if (isActivated)
        {
            InvokeInteractStart();
        }
        else
        {
            InvokeInteractEnded();
        }
    }

    void Update()
    {
        
    }

    public void ChangeMaterials(Material material)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }

    public virtual void Hover()
    {
        //Activate Outline
        ChangeMaterials(HoverMaterial);
    }

    public virtual void StopHover()
    {
        //Deactivate Outline
        ChangeMaterials(BaseMaterial);
    }

    public virtual void InteractionStart()
    {
        if (!IsToggle)
        {
            InvokeInteractStart();
        }
        else
        {
            isActivated = !isActivated;
            if (isActivated)
            {
                InvokeInteractStart();
            }
            else
            {
                InvokeInteractEnded();
            }
        }

        isPressed = true;
    }

    public virtual void InteractionEnd()
    {
        if (!IsToggle)
        {
            InvokeInteractEnded();
        }
        isPressed = false;
    }

    protected virtual void InvokeInteractStart()
    {
        OnInteractStarted?.Invoke(this);
        OnInteractStartedEvent?.Invoke();
        foreach (AnimatorTriggerer anim in OnInteractStartedAnimations)
        {
            anim.TriggerAnimation();
        }
    }

    protected virtual void InvokeInteractEnded()
    {
        OnInteractEnded?.Invoke(this);
        OnInteractEndedEvent?.Invoke();
        foreach (AnimatorTriggerer anim in OnInteractEndedAnimations)
        {
            anim.TriggerAnimation();
        }
    }
}
