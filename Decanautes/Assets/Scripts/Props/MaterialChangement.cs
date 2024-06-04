using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialChangement
{
    public enum MaterialParameterType
    {
        Float,
        Color,
        Vector3,
    }
    public bool IsAnim = false;
    [ShowIf("IsAnim")]
    public bool StartAnimOnStart = false;
    [ShowIf("IsAnim")]
    public bool IsInRythm = false;
    [ShowIf("IsAnim")]
    public bool HasSound = false;
    [ShowIf("HasSound")]
    public List<AnimationSound> animationSounds = new List<AnimationSound>();

    public bool IsMaterialInstance = false;

    [ShowIf("IsMaterialInstance")]
    public MeshRenderer Renderer;
    [ShowIf("IsMaterialInstance")]
    public int MaterialIndex;
    [HideIf("IsMaterialInstance")]
    public Material Material;

    [HideIf("IsAnim"), PropertyRange(0, 1)]
    public float CurrentDelta;
    [ShowIf("IsAnim")]
    public float LerpDuration;

    public string ParameterName;
    public MaterialParameterType ParameterType;

    [HideInInspector]
    public float ParameterFloatValueInit;
    [ShowIf("ParameterType", MaterialParameterType.Float), Tooltip("Ignore this if is anim")]
    public float ParameterFloatValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Float)]
    public float ParameterFloatValueEnd;


    [HideInInspector]
    public Color ParameterColorValueInit;
    [ShowIf("ParameterType", MaterialParameterType.Color), Tooltip("Ignore this if is anim")]
    public Color ParameterColorValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public Color ParameterColorValueEnd;
    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public float ParameterColorValueStartMult;
    [ShowIf("ParameterType", MaterialParameterType.Color)]
    public float ParameterColorValueEndMult;

    [HideInInspector]
    public Vector3 ParameterVector3ValueInit;
    [ShowIf("ParameterType", MaterialParameterType.Vector3), Tooltip("Ignore this if is anim")]
    public Vector3 ParameterVector3ValueStart;
    [ShowIf("ParameterType", MaterialParameterType.Vector3)]
    public Vector3 ParameterVector3ValueEnd;

    public void ChangeMaterialFromDelta(float delta)
    {
        CurrentDelta = delta;
        switch (ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    Color lerpedColor = Color.Lerp(ParameterColorValueStart, ParameterColorValueEnd, delta);
                    Material.SetColor(ParameterName, lerpedColor);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    float lerpedFloat = Mathf.Lerp(ParameterFloatValueStart, ParameterFloatValueEnd, delta);

                    Material.SetFloat(ParameterName, lerpedFloat);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    Vector3 lerpedVector = Vector3.Lerp(ParameterVector3ValueStart, ParameterVector3ValueEnd, delta);

                    Material.SetVector(ParameterName, lerpedVector);
                    break;
                }
        }
    }

    public void ChangeMaterialFromDelta()
    {
        switch (ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    Color lerpedColor = Color.Lerp(ParameterColorValueStart, ParameterColorValueEnd, CurrentDelta);
                    Material.SetColor(ParameterName, lerpedColor);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    float lerpedFloat = Mathf.Lerp(ParameterFloatValueStart, ParameterFloatValueEnd, CurrentDelta);

                    Material.SetFloat(ParameterName, lerpedFloat);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    Vector3 lerpedVector = Vector3.Lerp(ParameterVector3ValueStart, ParameterVector3ValueEnd, CurrentDelta);

                    Material.SetVector(ParameterName, lerpedVector);
                    break;
                }
        }
    }

    public void SetMaterialBackToInitialState()
    {
        if (IsMaterialInstance) return;
        switch (ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    Material.SetColor(ParameterName, ParameterColorValueInit);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    Material.SetFloat(ParameterName, ParameterFloatValueInit);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    Material.SetVector(ParameterName, ParameterVector3ValueInit);
                    break;
                }
        }
    }

    public void SetMaterialInitialState()
    {
        if (IsMaterialInstance) return;
        switch (ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    ParameterColorValueInit = Material.GetColor(ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    ParameterFloatValueInit = Material.GetFloat(ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    ParameterVector3ValueInit = Material.GetVector(ParameterName);
                    break;
                }
        }
    }

    public void SetMaterialStartValues()
    {
        switch (ParameterType)
        {
            case MaterialChangement.MaterialParameterType.Color:
                {
                    ParameterColorValueStart = Material.GetColor(ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Float:
                {
                    ParameterFloatValueStart = Material.GetFloat(ParameterName);
                    break;
                }
            case MaterialChangement.MaterialParameterType.Vector3:
                {
                    ParameterVector3ValueStart = Material.GetVector(ParameterName);
                    break;
                }
        }
    }

}
