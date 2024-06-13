using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
    public float Countdown;
    public ParameterType triggerType;
    public bool IsInRythm = false;
    public bool HasSound = false;
    [ShowIf("HasSound")]
    public List<AnimationSound> animationSounds = new List<AnimationSound>();
    


    public string parameterName;
    [ShowIf("triggerType", ParameterType.Bool)]
    public bool StateToApply;
    [ShowIf("triggerType", ParameterType.Float)]
    public float FloatToApply;
    [ShowIf("triggerType", ParameterType.Vector3)]
    public Vector3 Vector3ToApply;

    public UnityEvent OnAnimationFirstLooped;


    public IEnumerator TriggerAnimationAfterSeconds(float seconds)
    {   
        yield return new WaitForSecondsRealtime(seconds);

        if (HasSound)
        {
            foreach (var sound in animationSounds)
            {
                if (!sound.DoLaunchSoundAfterAnimation)
                    RythmManager.Instance.StartFmodEvent(sound.FmodEvent);
            }
        }
        if (IsInRythm)
        {
            RythmManager.Instance.OnBeatTrigger.AddListener(StartAnim);
            
        }
        else
        {
            StartAnim();

        }

    }

    public void TriggerAnimation()
    {
        if (HasSound)
        {
            foreach (var sound in animationSounds)
            {
                if (!sound.DoLaunchSoundAfterAnimation)
                    RythmManager.Instance.StartFmodEvent(sound.FmodEvent);
            }
        }
        if (IsInRythm)
        {
            RythmManager.Instance.OnBeatTrigger.AddListener(StartAnim);
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
                    parent.StartCoroutine(StartAndResetTrigger(animator, parameterName));
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
            if (HasSound)
            {
                foreach (var sound in animationSounds)
                {
                    if (sound.DoLaunchSoundAfterAnimation)
                    {
                        RythmManager.Instance.StartFmodEvent(sound.FmodEvent);
                    }
                }
            }
        }));
        return;
    }

    private IEnumerator StartAndResetTrigger(Animator animator, string TriggName)
    {
        animator.SetTrigger(TriggName);
        yield return new WaitForNextFrameUnit();
        animator.ResetTrigger(TriggName);
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

[Serializable]
public class AnimationSound
{
    public bool DoLaunchSoundAfterAnimation;
    public FmodEventInfo FmodEvent;
}