using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Decanautes.Interactable;

public class PropAnimation : MonoBehaviour
{
    public List<AnimatorTriggerer> Animations = new List<AnimatorTriggerer>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (AnimatorTriggerer animatorTriggerer in Animations)
        {
            animatorTriggerer.parent = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Button]
    public void TriggerAllAnimations()
    {
        foreach (AnimatorTriggerer anim in Animations)
        {
            

            if (anim.Countdown != 0)
            {
                StartCoroutine(anim.TriggerAnimationAfterSeconds(anim.Countdown));
            }
            else
            {
                anim.TriggerAnimation();
            }
        }



    }

    [Button]
    public void TriggerIndexAnim(int index)
    {
        if (Animations[index].Countdown != 0)
            StartCoroutine(Animations[index].TriggerAnimationAfterSeconds(Animations[index].Countdown));
        else
            Animations[index].TriggerAnimation();
    }

}
