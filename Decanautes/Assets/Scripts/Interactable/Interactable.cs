using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using FMODUnity;
using static UnityEditor.Profiling.RawFrameDataView;

namespace Decanautes.Interactable
{
    public class Interactable : MonoBehaviour
    {
        //--------Components
        [TitleGroup("Components")]
        public List<Renderer> renderers;
        public List<SelectionColor> SelectionColors;
        public Material BaseMaterial;
        public Color HoverColor;
        public Color ActiveColor;

        [TitleGroup("Parameters")]
        public bool IsToggle = false;
        public bool NeedLookToKeepInteraction = true;
        [Tooltip("It will wait for the first animation trigerrer's next animation end.")]
        public bool DoApplyStateAfterAnimation = true;


        //--------Events
        [TitleGroup("Events")]
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
            foreach (AnimatorTriggerer animatorTriggerer in OnInteractStartedAnimations)
            {
                animatorTriggerer.parent = this;
            }
            foreach (AnimatorTriggerer animatorTriggerer in OnInteractEndedAnimations)
            {
                animatorTriggerer.parent = this;
            }

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
            //ChangeMaterials(HoverMaterial);
            foreach (SelectionColor selectionColor in SelectionColors)
            {
                selectionColor.selectionColor = HoverColor;
                selectionColor.SetColor();
            }
        }

        public virtual void StopHover()
        {
            //Deactivate Outline
            //ChangeMaterials(BaseMaterial);
            foreach (SelectionColor selectionColor in SelectionColors)
            {
                selectionColor.selectionColor = new Color(0,0,0,0);
                selectionColor.SetColor();
            }
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
            foreach (AnimatorTriggerer anim in OnInteractStartedAnimations)
            {
                anim.TriggerAnimation();
            }
            if (DoApplyStateAfterAnimation && OnInteractStartedAnimations.Count > 0)
            {
                OnInteractStartedAnimations[0].OnAnimationFirstLooped.AddListener(() =>
                {
                    OnInteractStarted?.Invoke(this);
                    OnInteractStartedEvent?.Invoke();
                });
                return;
            }
            OnInteractStarted?.Invoke(this);
            OnInteractStartedEvent?.Invoke();
        }

        protected virtual void InvokeInteractEnded()
        {
            foreach (AnimatorTriggerer anim in OnInteractEndedAnimations)
            {
                anim.TriggerAnimation();
            }
            if (DoApplyStateAfterAnimation && OnInteractEndedAnimations.Count > 0)
            {
                OnInteractEndedAnimations[0].OnAnimationFirstLooped.AddListener(() =>
                {
                    OnInteractEnded?.Invoke(this);
                    OnInteractEndedEvent?.Invoke();
                });
                return;
            }
            OnInteractEnded?.Invoke(this);
            OnInteractEndedEvent?.Invoke();
        }

    }
}

