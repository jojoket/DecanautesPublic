using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Decanautes.Interactable
{
    public class Lever : Interactable
    {
        // Start is called before the first frame update
        void Start()
        {
            IsToggle = true;
            if (isActivated)
            {
                InvokeInteractStart();
            }
        }

        // Update is called once per frame
        void Update()
        {
            IsToggle = true;
        }

        public override void InteractionStart()
        {
            base.InteractionStart();
        }

        protected override void InvokeInteractStart()
        {
            base.InvokeInteractStart();

        }

        protected override void InvokeInteractEnded()
        {
            base.InvokeInteractEnded();
        }

        public override void InteractionEnd()
        {
            base.InteractionEnd();
        }

    }

}
