using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using JetBrains.Annotations;
using UnityEditor.ShaderGraph.Internal;
using Sirenix.Utilities;

[Serializable]
public class MaterialChangement
{
    public enum MaterialParameterType
    {
        Float,
        Color,
        Vector3,
    }

    public bool IsMaterialInstance = false;

    [ShowIf("IsMaterialInstance")]
    public MeshRenderer Renderer;
    [ShowIf("IsMaterialInstance")]
    public int MaterialIndex;
    [HideIf("IsMaterialInstance")]
    public Material Material;

    [Unit(Units.Second)]
    public float LerpDuration;
    public string ParameterName;
    public MaterialParameterType ParameterType;

    [HideInInspector]
    public float ParameterFloatValueInit;
    [HideInInspector]
    public float ParameterFloatValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Float)]
    public float ParameterFloatValueEnd;


    [HideInInspector]
    public Color ParameterColorValueInit;
    [HideInInspector]
    public Color ParameterColorValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public Color ParameterColorValueEnd;
    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public float ParameterColorValueStartMult;
    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public float ParameterColorValueEndMult;

    [HideInInspector]
    public Vector3 ParameterVector3ValueInit;
    [HideInInspector]
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
        foreach (var materialChangement in materialChangements)
        {
            SetMaterialInitialState(materialChangement);
        }
    }

    private void OnDestroy()
    {
        foreach (var materialChangement in materialChangements)
        {
            SetMaterialBackToInitialState(materialChangement);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    [Button]
    public void TriggerAllAnimations()
    {
        foreach (var materialChangement in materialChangements)
        {
            StartCoroutine(StartAnimation(materialChangement));
        }
    }

    [Button]
    public void TriggerAnimationByIndex(int index)
    {
        StartCoroutine(StartAnimation(materialChangements[index]));
    }


    private IEnumerator StartAnimation(MaterialChangement materialChangement)
    {
        float startTime = Time.time;
        float endTime = Time.time + materialChangement.LerpDuration;

        if (materialChangement.IsMaterialInstance)
        {
            materialChangement.Material = materialChangement.Renderer.materials[materialChangement.MaterialIndex];
        }
        SetMaterialStartValues(materialChangement);
        float delta = 0;
        while (Time.time < endTime)
        {
            delta += Time.deltaTime / materialChangement.LerpDuration;
            ChangeMaterialFromDelta(materialChangement, delta);
            yield return 0;
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
                    float lerpedFloat = Mathf.Lerp(materialChangement.ParameterFloatValueStart, materialChangement.ParameterFloatValueEnd, delta);

                    materialChangement.Material.SetFloat(materialChangement.ParameterName, lerpedFloat);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    Vector3 lerpedVector = Vector3.Lerp(materialChangement.ParameterVector3ValueStart, materialChangement.ParameterVector3ValueEnd, delta);

                    materialChangement.Material.SetVector(materialChangement.ParameterName, lerpedVector);
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
                    materialChangement.ParameterFloatValueStart = materialChangement.Material.GetFloat(materialChangement.ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    materialChangement.ParameterVector3ValueStart = materialChangement.Material.GetVector(materialChangement.ParameterName);
                    break;
                }
        }
    }

    public void SetMaterialBackToInitialState(MaterialChangement materialChangement)
    {
        if (materialChangement.IsMaterialInstance) return;
        switch (materialChangement.ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    materialChangement.Material.SetColor(materialChangement.ParameterName, materialChangement.ParameterColorValueInit);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    materialChangement.Material.SetFloat(materialChangement.ParameterName, materialChangement.ParameterFloatValueInit);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    materialChangement.Material.SetVector(materialChangement.ParameterName, materialChangement.ParameterVector3ValueInit);
                    break;
                }
        }
    }

    public void SetMaterialInitialState(MaterialChangement materialChangement)
    {
        if (materialChangement.IsMaterialInstance) return;
        switch (materialChangement.ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    materialChangement.ParameterColorValueInit =  materialChangement.Material.GetColor(materialChangement.ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    materialChangement.ParameterFloatValueInit = materialChangement.Material.GetFloat(materialChangement.ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    materialChangement.ParameterVector3ValueInit = materialChangement.Material.GetVector(materialChangement.ParameterName);
                    break;
                }
        }
    }

}
