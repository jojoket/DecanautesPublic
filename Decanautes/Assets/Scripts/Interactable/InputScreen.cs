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
        [TitleGroup("Components")]
        public Renderer ScreenRenderer;
        [TitleGroup("Components")]
        public List<Animation> ButtonsAnimations;
        [TitleGroup("Paramaters")]
        public bool DoStopMovementsWhenIsEditing;
        [TitleGroup("Paramaters")]
        public string CodeNeeded;
        [HideInInspector]
        public bool IsEditing;
        public Material NormalColor;
        public Material InvalidColor;
        public Material ValidColor;

        [TitleGroup("Events")]
        public UnityEvent OnStartEditing;
        [TitleGroup("Events")]
        public UnityEvent OnEndEditing;
        [TitleGroup("Events")]
        public UnityEvent OnCodeValid;
        [TitleGroup("Events")]
        public UnityAction<Interactable> OnCodeValidAction;
        [TitleGroup("Events")]
        public UnityEvent OnCodeInvalid;
        [TitleGroup("Events")]
        public UnityAction<Interactable> OnCodeInvalidAction;


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

        [Button]
        public bool SelectText()
        {
            InputField.Select();
            IsEditing = true;
            int lastLength = InputField.text.Length;
            InputField.onValueChanged.AddListener((string str) =>
            {
                if (str == ">" || str == "<")
                    return;
                if (str.Length == 0 || lastLength > str.Length)
                {
                    lastLength = str.Length;
                    return;
                }
                lastLength = str.Length;
                bool isNum = int.TryParse(str[str.Length - 1].ToString(), out int num);
                
                if (!isNum)
                {
                    if (InputField.text.Length != 0)
                    {
                        InputField.text = InputField.text.Remove(InputField.text.Length-1);
                    }
                    return;
                }
                NumPadInput(num);
            });
            return true;
        }

        public void DeselectText()
        {
            InputField.ReleaseSelection();
            IsEditing = false;
            CheckForCodeValidity();
        }

        public void NumPadInput(int num)
        {
            ButtonsAnimations[num].Play();
        }


        private void CheckForCodeValidity()
        {
            if (InputField.text == CodeNeeded)
            {
                InputField.text = "";
                OnCodeValid?.Invoke();
                OnCodeValidAction?.Invoke(this);
                StartCoroutine(CodeMatVisual(ValidColor));
                return;
            }
            if (int.TryParse(InputField.text, out int res))
            {
                if (res < int.Parse(CodeNeeded))
                {
                    InputField.text = ">";
                }
                else if (res > int.Parse(CodeNeeded))
                {
                    InputField.text = "<";
                }
            }
            OnCodeInvalid?.Invoke();
            OnCodeInvalidAction?.Invoke(this);
            StartCoroutine(CodeMatVisual(InvalidColor));
        }

        private IEnumerator CodeMatVisual(Material colorVisual)
        {
            foreach (Animation anims in ButtonsAnimations)
            {
                anims.transform.GetChild(0).GetComponent<MeshRenderer>().material = colorVisual;
            }
            ScreenRenderer.material = colorVisual;
            yield return new WaitForSecondsRealtime(0.75f);
            InputField.text = "";
            foreach (Animation anims in ButtonsAnimations)
            {
                anims.transform.GetChild(0).GetComponent<MeshRenderer>().material = NormalColor;
            }
            ScreenRenderer.material = NormalColor;
        }

    }
}
