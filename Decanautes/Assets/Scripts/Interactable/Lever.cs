using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Decanautes.Interactable
{
    public class Lever : Interactable
    {
        public Animation LeverAnimation;
        public AnimationClip[] LeverAnimationClips = new AnimationClip[2];

        // Start is called before the first frame update
        void Start()
        {
            IsToggle = true;
            if (isActivated)
            {
                InvokeInteractStart();
            }
            else
            {
                InvokeInteractEnded();
            }
        }

        // Update is called once per frame
        void Update()
        {
            IsToggle = true;
        }

        public override void InteractionStart()
        {
            if (LeverAnimation.isPlaying)
            {
                return;
            }
            base.InteractionStart();
        }

        protected override void InvokeInteractStart()
        {
            base.InvokeInteractStart();
            LeverAnimation.Play(LeverAnimationClips[0].name);

        }

        protected override void InvokeInteractEnded()
        {
            base.InvokeInteractEnded();
            LeverAnimation.Play(LeverAnimationClips[1].name);
        }

        public override void InteractionEnd()
        {
            base.InteractionEnd();
        }

    }

}
