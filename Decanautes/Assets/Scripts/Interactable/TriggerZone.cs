using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    private Collider _collider;
    public LayerMask LayerToDetect;
    public bool DoExitOnDisable = true;
    public UnityEvent OnEnterZone;
    public UnityEvent OnExitZone;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        if (DoExitOnDisable)
        {
            OnExitZone?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((LayerToDetect & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }
        OnEnterZone?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if ((LayerToDetect & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }
        OnExitZone?.Invoke();
    }
}
