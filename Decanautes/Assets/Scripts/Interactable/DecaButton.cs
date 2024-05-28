using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decanautes.Interactable
{
    public class DecaButton : Interactable
    {


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
        }

        void Update()
        {

        }


        public override void InteractionStart()
        {
            base.InteractionStart();
        }

        public override void InteractionEnd()
        {
            base.InteractionEnd();
        }
    }
}
