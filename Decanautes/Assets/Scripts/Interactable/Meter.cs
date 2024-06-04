using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

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
        [TabGroup("Property")]
        public List<MaterialChangement> materialChangements;


        private Vector3 baseScale;
        private Vector3 basePosition;
        private Quaternion baseRotation;

        // Start is called before the first frame update
        void Start()
        {
            if (Indicator != null)
            {
                baseScale = Indicator.transform.localScale;
                basePosition = Indicator.transform.position;
                baseRotation = Indicator.transform.rotation;
                IndicatorRenderer = Indicator.GetComponent<MeshRenderer>();
                material = Instantiate<Material>(IndicatorRenderer.material);
                IndicatorRenderer.material = material;
            }
            foreach (var materialChangement in materialChangements)
            {
                materialChangement.SetMaterialInitialState();
            }
        }

        private void OnDestroy()
        {
            foreach (var materialChangement in materialChangements)
            {
                materialChangement.SetMaterialBackToInitialState();
            }
        }

        // Update is called once per frame
        void Update()
        {
            FillVisual();
        }


        private void FillVisual()
        {
            if (Indicator != null)
            {
                Vector3 currentFill = FillAmount * FillSizeMax;
                Indicator.transform.localScale = baseScale + currentFill;
                Indicator.transform.position = basePosition + new Vector3(currentFill.x * FillDirection.x/2, currentFill.y * FillDirection.y/2, currentFill.z * FillDirection.z/2) + TranslationMax * FillAmount;

                Indicator.transform.rotation = Quaternion.Lerp(baseRotation, Quaternion.Euler(RotationMax), FillAmount);
            }

            foreach (var materialChangement in materialChangements)
            {
                materialChangement.ChangeMaterialFromDelta(FillAmount);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Indicator != null)
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
}
