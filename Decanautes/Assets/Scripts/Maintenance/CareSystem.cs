using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

[Serializable]
public class Maintain
{
    [TabGroup("Components")]
    public MaintainableData MaintainableData;
    [TabGroup("Components")]
    public Meter MaintainableMeter;
    [TabGroup("Components")]
    public Interactable interactableFiller;
    [TabGroup("Parameters")]
    public List<GameObject> ToEnableOnUnderThreshold;
    [TabGroup("Parameters")]
    public List<GameObject> ToDisableOnUnderThreshold;
    [TabGroup("Parameters")]
    public UnityEvent OnUnderThreshold;
    [TabGroup("Parameters")]
    public UnityEvent OnOverThreshold;
    [TabGroup("Parameters")]
    public List<GameObject> ToEnableOnZero;
    [TabGroup("Parameters")]
    public List<GameObject> ToDisableOnZero;
    [TabGroup("Parameters")]
    public UnityEvent OnZero;
    [TabGroup("Parameters")]
    public UnityEvent OnOverZero;
    [TabGroup("Debug"), ReadOnly]
    public bool isOverThreshold;
    [TabGroup("Debug"), ReadOnly]
    public bool isZero;
}

public class CareSystem : MonoBehaviour
{

    [SerializeField]
    private bool _doSave;

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
            for (int i = 0; i < Maintainables.Count; i++)
            {
                Destroy(initialState[i]);
            }
            return;
        }
        for (int i = 0; i < Maintainables.Count; i++)
        {
            Maintainables[i].MaintainableData.CurrentState = initialState[i].CurrentState;
            Maintainables[i].interactableFiller.OnInteractStarted -= FillMaintainable;
            Destroy(initialState[i]);
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
        if (maintain.MaintainableData.CurrentState < maintain.MaintainableData.Threshold && maintain.isOverThreshold)
        {
            OnUnderThreshold(maintain);
        }
        if (maintain.MaintainableData.CurrentState > maintain.MaintainableData.Threshold && !maintain.isOverThreshold)
        {
            OnOverThreshold(maintain);
        }
        if (maintain.MaintainableData.CurrentState <= 0 && !maintain.isZero)
        {
            OnZero(maintain);
        }
        if (maintain.MaintainableData.CurrentState > 0 && maintain.isZero)
        {
            OnOverZero(maintain);
        }
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
        float fillTimeSec = interactable.LinkedMaintainable.MaintainableData.FillingTime;
        DOTween.To(() => interactable.LinkedMaintainable.MaintainableData.CurrentState, x => interactable.LinkedMaintainable.MaintainableData.CurrentState = x, 1, fillTimeSec);
    }

    private void OnUnderThreshold(Maintain maintain)
    {
        maintain.isOverThreshold = false;
        maintain.OnUnderThreshold?.Invoke();
        foreach (GameObject item in maintain.ToDisableOnUnderThreshold)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in maintain.ToEnableOnUnderThreshold)
        {
            item.SetActive(true);
        }
    }
    private void OnOverThreshold(Maintain maintain)
    {
        maintain.isOverThreshold = true;
        maintain.OnOverThreshold?.Invoke();
        foreach (GameObject item in maintain.ToDisableOnUnderThreshold)
        {
            item.SetActive(true);
        }
        foreach (GameObject item in maintain.ToEnableOnUnderThreshold)
        {
            item.SetActive(false);
        }
    }

    private void OnZero(Maintain maintain)
    {
        maintain.isZero = true;
        maintain.OnZero?.Invoke();
        foreach (GameObject item in maintain.ToDisableOnZero)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in maintain.ToEnableOnZero)
        {
            item.SetActive(true);
        }
    }

    private void OnOverZero(Maintain maintain)
    {
        maintain.isZero = false;
        maintain.OnOverZero?.Invoke();
        foreach (GameObject item in maintain.ToDisableOnZero)
        {
            item.SetActive(true);
        }
        foreach (GameObject item in maintain.ToEnableOnZero)
        {
            item.SetActive(false);
        }
    }
}
