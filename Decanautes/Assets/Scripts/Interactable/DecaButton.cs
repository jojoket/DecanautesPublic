using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decanautes.Interactable
{
    public class DecaButton : Interactable
    {
        public Animation pressAnimation;


        void Start()
        {
        
        }

        void Update()
        {
        
        }


        public override void InteractionStart()
        {
            base.InteractionStart();
            pressAnimation.Play();
        }

        public override void InteractionEnd()
        {
            base.InteractionEnd();
            pressAnimation.Play("ButtonReleased");
        }
    }
}