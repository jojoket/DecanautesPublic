using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using JetBrains.Annotations;
using UnityEditor.ShaderGraph.Internal;

[Serializable]
public class MaterialChangement
{
    public enum MaterialParameterType
    {
        Float,
        Color,
        Vector3,
    }
    public Material Material;
    [Unit(Units.Second)]
    public float LerpDuration;
    public string ParameterName;
    public MaterialParameterType ParameterType;

    [HideInInspector]
    public float ParameterFloatValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Float)]
    public float ParameterFloatValueEnd;

    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public Color ParameterColorValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public Color ParameterColorValueEnd;

    [ShowIf("ParameterType", MaterialParameterType.Vector3)]
    public Vector3 ParameterVector3ValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Vector3)]
    public Vector3 ParameterVector3ValueEnd;
}


public class MaterialAnimation : MonoBehaviour
{
    public List<MaterialChangement> materialChangements;

    


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
        foreach (var materialChangement in materialChangements)
        {
            StartCoroutine(StartAnimation(materialChangement));
        }
    }

    public void TriggerAnimationByIndex(int index)
    {
        StartCoroutine(StartAnimation(materialChangements[index]));
    }


    private IEnumerator StartAnimation(MaterialChangement materialChangement)
    {
        float startTime = Time.time;
        float endTime = Time.time + materialChangement.LerpDuration;
        SetMaterialStartValues(materialChangement);
        while (Time.time < endTime)
        {
            float delta = Time.deltaTime / materialChangement.LerpDuration;
            float lerpDelta = Time.deltaTime / ;
            ChangeMaterialFromDelta(materialChangement, delta);
            yield return new WaitForEndOfFrame();
        }
    }

    private void ChangeMaterialFromDelta(MaterialChangement materialChangement, float delta)
    {
        switch(materialChangement.ParameterType) {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    Color lerpedColor = Color.Lerp(materialChangement.ParameterColorValueStart, materialChangement.ParameterColorValueEnd, delta);

                    materialChangement.Material.SetColor(materialChangement.ParameterName, lerpedColor);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    break;
                }
        }
    }

    private void SetMaterialStartValues(MaterialChangement materialChangement)
    {
        switch (materialChangement.ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    materialChangement.ParameterColorValueStart = materialChangement.Material.GetColor(materialChangement.ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    materialChangement.ParameterFloatValueEnd = materialChangement.Material.GetFloat(materialChangement.ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    materialChangement.ParameterVector3ValueEnd = materialChangement.Material.GetVector(materialChangement.ParameterName);
                    break;
                }
        }
    }

}
