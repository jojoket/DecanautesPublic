using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RythmManager : MonoBehaviour
{
    public RythmManagerData RythmManagerData;

    public UnityEvent OnQuarterNoteTrigger;
    public UnityEvent OnHalfQuarterNoteTrigger;
    public UnityEvent OnDoubleQuarterNoteTrigger;

    //(une noire en français)
    private float _quarterNoteRythmInMs;
    //(une croche en français)
    private float _halfQuarterNoteRythmInMs;
    //(uen blanche en français)
    private float _doubleQuarterNoteRythmInMs;

    private float _currentQuarterNoteTime;
    private float _currentHalfQuarterNoteTime;
    private float _currentDoubleQuarterNoteTime;

    // Start is called before the first frame update
    void Start()
    {
        _quarterNoteRythmInMs = 60000 / RythmManagerData.Bpm;
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

}
