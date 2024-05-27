using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Decanautes.Interactable
{
    

    public class InputScreen : Interactable
    {
        [TitleGroup("Components")]
        public TMP_InputField InputField;
        [TitleGroup("Paramaters")]
        public bool DoStopMovementsWhenIsEditing;

        [TitleGroup("Events")]
        public UnityEvent OnStartEditing;
        public UnityEvent OnEndEditing;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
