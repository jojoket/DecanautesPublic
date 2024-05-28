using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using FMODUnity;

namespace Decanautes.Interactable
{
    [Serializable]
    public class AnimatorTriggerer
    {
        [HideInInspector]
        public MonoBehaviour parent;
        public Animator animator;
        public enum ParameterType
        {
            Trigger,
            Bool,
            Float,
            Vector3,
        }
        public ParameterType triggerType;
        public bool IsInRythm = false;
        public bool HasSound = false;
        [ShowIf("HasSound")]
        public bool DoLaunchSoundAfterAnimation;
        [ShowIf("HasSound")]
        public EventReference EventPath;
        
        
        public string parameterName;
        [ShowIf("triggerType", ParameterType.Bool)]
        public bool StateToApply;
        [ShowIf("triggerType", ParameterType.Float)]
        public float FloatToApply;
        [ShowIf("triggerType", ParameterType.Vector3)]
        public Vector3 Vector3ToApply;

        public UnityEvent OnAnimationFirstLooped;

        public void TriggerAnimation()
        {
            if (IsInRythm)
            {
                RythmManager.Instance.OnBeatTrigger.AddListener(StartAnim);
                if (HasSound && !DoLaunchSoundAfterAnimation)
                {
                    RythmManager.Instance.AddFModEventToBuffer(EventPath);
                }
                return;
            }

            StartAnim();
        }

        private void StartAnim()
        {
            RythmManager.Instance.OnBeatTrigger.RemoveListener(StartAnim);

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
            parent.StartCoroutine(CheckForAnimationEnd(animator, () =>
            {
                OnAnimationFirstLooped?.Invoke();
                OnAnimationFirstLooped.RemoveAllListeners();
                if (DoLaunchSoundAfterAnimation && HasSound)
                {
                    RythmManager.Instance.AddFModEventToBuffer(EventPath);
                }
            }));
            return;
        }
        private IEnumerator CheckForAnimationEnd(Animator animator, Action callBack)
        {
            bool animStarted = false;
            while (animator != null)
            {
                if (!animStarted && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
                {
                    animStarted = true;
                }
                if (animStarted && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    callBack();
                    break;
                }
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }

    }

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

