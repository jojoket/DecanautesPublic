using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Decanautes.Interactable
{
    public class Meter : MonoBehaviour
    {
        [TabGroup("Components")]
        public GameObject Indicator;
        [TabGroup("Components")]
        public MeshRenderer IndicatorRenderer;
        [HideInInspector]
        public Material material;

        [TabGroup("Property")]
        [PropertyRange(0,1)]
        public float FillAmount;
        [TabGroup("Property")]
        public Vector3 FillSizeMax;
        [TabGroup("Property")]
        public Vector3 FillDirection;
        [TabGroup("Property")]
        public Vector3 RotationMax;
        [TabGroup("Property")]
        public Vector3 TranslationMax;

        private Vector3 baseScale;
        private Vector3 basePosition;
        private Quaternion baseRotation;

        // Start is called before the first frame update
        void Start()
        {
            baseScale = Indicator.transform.localScale;
            basePosition = Indicator.transform.position;
            baseRotation = Indicator.transform.rotation;
            IndicatorRenderer = Indicator.GetComponent<MeshRenderer>();
            material = Instantiate<Material>(IndicatorRenderer.material);
            IndicatorRenderer.material = material;
        }

        // Update is called once per frame
        void Update()
        {
            FillVisual();
        }


        private void FillVisual()
        {
            Vector3 currentFill = FillAmount * FillSizeMax;
            Indicator.transform.localScale = baseScale + currentFill;
            Indicator.transform.position = basePosition + new Vector3(currentFill.x * FillDirection.x/2, currentFill.y * FillDirection.y/2, currentFill.z * FillDirection.z/2) + TranslationMax * FillAmount;

            Indicator.transform.rotation = Quaternion.Lerp(baseRotation, Quaternion.Euler(RotationMax), FillAmount);
        }

        private void OnDrawGizmosSelected()
        {
            Mesh mesh = Indicator.GetComponent<MeshFilter>().sharedMesh;
            Vector3 currentFill = FillAmount * FillSizeMax;
            Vector3 scale = Indicator.transform.localScale + currentFill;
            Vector3 pos = Indicator.transform.position + new Vector3(currentFill.x * FillDirection.x/2, currentFill.y * FillDirection.y/2, currentFill.z * FillDirection.z/2) + TranslationMax * FillAmount;
            Quaternion rot = Quaternion.Lerp(Indicator.transform.rotation, Quaternion.Euler(RotationMax), FillAmount);
            Gizmos.DrawWireMesh(mesh, pos, rot, scale);
        }

    }
}
