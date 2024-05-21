using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decanautes.Interactable;

public class PropAnim : MonoBehaviour
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

    public void TriggerAllAnimations()
    {
        foreach (AnimatorTriggerer anim in Animations)
        {
            anim.TriggerAnimation();
        }


    }

}
