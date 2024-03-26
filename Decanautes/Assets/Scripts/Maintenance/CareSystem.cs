using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class Maintain
{
    public MaintainableData MaintainableData;
    public Meter MaintainableMeter;
    public Interactable interactableFiller;
}

public class CareSystem : MonoBehaviour
{

    [SerializeField]
    private bool _doSave;
    [TableList]
    public List<Maintain> Maintainables = new List<Maintain>();

    private List<MaintainableData> initialState = new List<MaintainableData>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Maintain item in Maintainables)
        {
            MaintainableData data = Instantiate<MaintainableData>(item.MaintainableData);
            initialState.Add(data);
            item.interactableFiller.LinkedMaintainable = item;
            item.interactableFiller.OnInteractStarted += FillMaintainable;
        }
    }

    private void OnDestroy()
    {
        if (_doSave)
        {
            return;
        }
        for (int i = 0; i < Maintainables.Count; i++)
        {
            Maintainables[i].MaintainableData.CurrentState = initialState[i].CurrentState;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Maintain item in Maintainables)
        {
            ManageStates(item);
            UpdateMeterVisuals(item);
        }
    }

    private void ManageStates(Maintain maintain)
    {
        maintain.MaintainableData.CurrentState += maintain.MaintainableData.Speed * Time.deltaTime;
        maintain.MaintainableData.CurrentState = Math.Clamp(maintain.MaintainableData.CurrentState,0,1);
    }

    private void UpdateMeterVisuals(Maintain maintain)
    {
        maintain.MaintainableMeter.FillAmount = maintain.MaintainableData.CurrentState;
        if (maintain.MaintainableData.CurrentState <= maintain.MaintainableData.Threshold)
        {
            maintain.MaintainableMeter.fillRenderer.material.color = maintain.MaintainableData.warningColor; //.SetColor("_Color", maintain.MaintainableData.warningColor);
        }
        else
        {
            maintain.MaintainableMeter.fillRenderer.material.color = maintain.MaintainableData.baseColor; //.SetColor("_Color", maintain.MaintainableData.baseColor);
        }
    }

    private void FillMaintainable(Interactable interactable)
    {
        bool needTresholdToMaintain = interactable.LinkedMaintainable.MaintainableData.NeedThresholdToMaintain;
        float currentState = interactable.LinkedMaintainable.MaintainableData.CurrentState;
        float threshold = interactable.LinkedMaintainable.MaintainableData.Threshold;
        if (needTresholdToMaintain && currentState > threshold)
        {
            return;
        }
        interactable.LinkedMaintainable.MaintainableData.CurrentState = 1;
    }
}
