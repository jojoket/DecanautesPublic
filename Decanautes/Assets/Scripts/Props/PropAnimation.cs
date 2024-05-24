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
            anim.TriggerAnimation();
        }


    }

    [Button]
    public void TriggerIndexAnim(int index)
    {
        Animations[index].TriggerAnimation();
    }

}
