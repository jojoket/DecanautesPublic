using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class RythmManager : MonoBehaviour
{
    //Singleton
    public static RythmManager Instance;

    public RythmManagerData RythmManagerData;

    public UnityEvent OnQuarterNoteTrigger;
    public UnityEvent OnHalfQuarterNoteTrigger;
    public UnityEvent OnDoubleQuarterNoteTrigger;

    //(une noire en français)
    [SerializeField, ReadOnly] private float _quarterNoteRythmInMs;
    //(une croche en français)
    [SerializeField, ReadOnly] private float _halfQuarterNoteRythmInMs;
    //(une blanche en français)
    [SerializeField, ReadOnly] private float _doubleQuarterNoteRythmInMs;

    [SerializeField, ReadOnly]
    private float _currentQuarterNoteTime;
    [SerializeField, ReadOnly]
    private float _currentHalfQuarterNoteTime;
    [SerializeField, ReadOnly]
    private float _currentDoubleQuarterNoteTime;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Rythm Manager already exist");
            return;
        }
        Instance = this;
        _quarterNoteRythmInMs = 60 / RythmManagerData.Bpm;
        _halfQuarterNoteRythmInMs = _quarterNoteRythmInMs / 2;
        _doubleQuarterNoteRythmInMs = _quarterNoteRythmInMs * 2;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        ResetTimersAndTriggerEvents();
    }

    private void UpdateTimers()
    {
        _currentQuarterNoteTime += Time.deltaTime;
        _currentHalfQuarterNoteTime += Time.deltaTime;
        _currentDoubleQuarterNoteTime += Time.deltaTime;
    }

    private void ResetTimersAndTriggerEvents()
    {
        if (_currentQuarterNoteTime >= _quarterNoteRythmInMs)
        {
            _currentQuarterNoteTime = 0;
            OnQuarterNoteTrigger?.Invoke();
        }
        if (_currentHalfQuarterNoteTime >= _halfQuarterNoteRythmInMs)
        {
            _currentHalfQuarterNoteTime = 0;
            OnHalfQuarterNoteTrigger?.Invoke();
        }
        if (_currentDoubleQuarterNoteTime >= _doubleQuarterNoteRythmInMs)
        {
            _currentDoubleQuarterNoteTime = 0;
            OnDoubleQuarterNoteTrigger?.Invoke();
        }
    }

    private void OnDestroy()
    {
        OnQuarterNoteTrigger.RemoveAllListeners();
        OnHalfQuarterNoteTrigger.RemoveAllListeners();
        OnDoubleQuarterNoteTrigger.RemoveAllListeners();
    }
}
