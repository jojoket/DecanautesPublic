using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeActivator : MonoBehaviour
{
    public CinemachineImpulseSource cinemachineImpulseSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerCameraShake()
    {
        cinemachineImpulseSource.GenerateImpulseWithForce(1);

    }
}
