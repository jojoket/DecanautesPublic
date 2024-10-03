using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using FMODUnity;
using DG.Tweening;

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
        public bool CanInteract = true;
        [ShowIf("IsToggle")]
        public bool CanOnlyOn = false;
        [ShowIf("IsToggle")]
        public bool CanOnlyOff = false;
        public bool NeedLookToKeepInteraction = true;
        [Tooltip("It will wait for the first animation trigerrer's next animation end.")]
        public bool DoApplyStateAfterAnimation = true;

        public float ResetAfter = -1;

        //--------Events
        [TitleGroup("Events")]
        public List<AnimatorTriggerer> OnInteractStartedAnimations = new List<AnimatorTriggerer>();
        public UnityAction<Interactable> OnInteractStarted;
        public UnityEvent OnInteractStartedEvent;
        public UnityEvent OnClickedEvent;
        public List<AnimatorTriggerer> OnInteractEndedAnimations = new List<AnimatorTriggerer>();
        public UnityAction<Interactable> OnInteractEnded;
        public UnityEvent OnInteractEndedEvent;
        public List<AnimatorTriggerer> OnTriedAnimations = new List<AnimatorTriggerer>();
        public UnityEvent OnTriedEvent;

        [HideInInspector]
        public Event LinkedEvent;
        [HideInInspector]
        public Maintain LinkedMaintainable;
        [TitleGroup("Debug")]
        public bool isActivated = false;
        [ReadOnly]
        public bool isPressed;
        public bool doResetPressedOnCannotInteract;



        void Start()
        {
            HoverColor = PlayerPreferencesManager.Instance.PlayerPreferencesData.OutlineColor;
            foreach (AnimatorTriggerer animatorTriggerer in OnInteractStartedAnimations)
            {
                animatorTriggerer.parent = this;
            }
            foreach (AnimatorTriggerer animatorTriggerer in OnInteractEndedAnimations)
            {
                animatorTriggerer.parent = this;
            }
            foreach (AnimatorTriggerer animatorTriggerer in OnTriedAnimations)
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
                if (selectionColor == null)
                    continue;
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
                if (selectionColor == null)
                    continue;
                selectionColor.selectionColor = new Color(0,0,0,0);
                selectionColor.SetColor();
            }
        }


        [Button]
        public virtual void InteractionStart()
        {
            if (!CanInteract)
            {
                foreach (AnimatorTriggerer anim in OnTriedAnimations)
                {
                    anim.TriggerAnimation();
                }
                OnTriedEvent?.Invoke();
                return;
            }
            if (IsToggle)
            {
                if (CanOnlyOff && !isActivated)
                    return;
                if (CanOnlyOn && isActivated)
                    return;
            }
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
                    if (ResetAfter > 0 && !CanOnlyOn)
                    {
                        DOVirtual.DelayedCall(ResetAfter, () =>
                        {
                            if (isActivated)
                                InteractionStart();
                        });
                    }
                }
                else
                {
                    InvokeInteractEnded();
                }
            }

            isPressed = true;
        }

        [Button]
        public virtual void InteractionEnd()
        {
            if (!CanInteract)
            {
                return;
            }
            if (!IsToggle)
            {
                InvokeInteractEnded();
            }
            isPressed = false;
        }

        public void SetCanInteract(bool canInteract)
        {
            if (isPressed && !doResetPressedOnCannotInteract)
            {
                InteractionEnd();
            }
            else if (doResetPressedOnCannotInteract)
            {
                isPressed = false;
            }
            CanInteract = canInteract;
        }

        public void SetCanOnlyOff(bool value)
        {
            CanOnlyOff = value;
        }
        public void SetCanOnlyOn(bool value)
        {
            CanOnlyOn = value;
        }

        protected virtual void InvokeInteractStart()
        {
            foreach (AnimatorTriggerer anim in OnInteractStartedAnimations)
            {
                anim.TriggerAnimation();
            }
            OnClickedEvent?.Invoke();
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
            OnClickedEvent?.Invoke();
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

