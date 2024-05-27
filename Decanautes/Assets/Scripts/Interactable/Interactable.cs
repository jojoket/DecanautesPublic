using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using FMODUnity;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Decanautes.Interactable
{
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
        public bool IsInRythm = false;
        public bool HasSound = false;
        [ShowIf("HasSound")]
        public EventReference EventPath;
        
        
        public string parameterName;
        [ShowIf("triggerType", ParameterType.Bool)]
        public bool StateToApply;
        [ShowIf("triggerType", ParameterType.Float)]
        public float FloatToApply;
        [ShowIf("triggerType", ParameterType.Vector3)]
        public Vector3 Vector3ToApply;

        public void TriggerAnimation()
        {
            if (IsInRythm)
            {
                RythmManager.Instance.OnBeatTrigger.AddListener(StartAnim);
                if (HasSound)
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
        public bool DoApplyStateAfterAnimation = true;


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
            if (DoApplyStateAfterAnimation)
            {
                Animator animator = OnInteractStartedAnimations[0].animator;
                StartCoroutine(CheckForAnimationEnd(animator, () =>
                {
                    OnInteractStarted?.Invoke(this);
                    OnInteractStartedEvent?.Invoke();
                }));
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
            if (DoApplyStateAfterAnimation)
            {
                Animator animator = OnInteractEndedAnimations[0].animator;
                StartCoroutine(CheckForAnimationEnd(animator, () =>
                {
                    OnInteractEnded?.Invoke(this);
                    OnInteractEndedEvent?.Invoke();
                }));
                return;
            }
            OnInteractEnded?.Invoke(this);
            OnInteractEndedEvent?.Invoke();
        }

        private IEnumerator CheckForAnimationEnd(Animator animator, Action callBack)
        {
            while (animator != null)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                {
                    callBack();
                    break;
                }
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

    }
}

