using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decanautes.Interactable
{
    [CreateAssetMenu(fileName = "Grab_", menuName = "ScriptableObject/GrabbableData")]
    public class GrabbableData : ScriptableObject
    {
        public float GrabRange;
        public float GrabForce;
        public bool isSimulated;
    }
}

