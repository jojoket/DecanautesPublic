using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


public class Interactable : MonoBehaviour
{
    //--------Components
    public List<Renderer> renderers;
    public Material BaseMaterial;
    public Material HoverMaterial;
    public Material ActiveMaterial;

    //--------Events
    public UnityAction<Interactable> OnInteractStarted;
    public UnityAction<Interactable> OnInteractEnded;
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
        isPressed = true;
    }

    public virtual void InteractionEnd()
    {
        OnInteractEnded?.Invoke(this);
        isPressed = false;
    }
}
