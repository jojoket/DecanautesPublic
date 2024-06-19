using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Experimental.GlobalIllumination;

public class script_Light_Day_Night : MonoBehaviour
{


    public bool DoDaySync = false;
    [ShowIf("DoDaySync")]
    public float StartHour = 8;
    [ShowIf("DoDaySync")]
    public float StartMinute = 0;
    [ShowIf("DoDaySync")]
    public float EndHour = 20;
    [ShowIf("DoDaySync")]
    public float EndMinute = 0;
    [ShowIf("DoDaySync")]
    public float currentSunT;



    public float lerpDuration = 10;
    float timeElapsed;

    public GameObject sun;
    public GameObject celestialBody;
    public Vector3 startDirection;
    public Vector3 endDirection;

    public Color sunStartColor;
    public Color sunEndColor;

    Vector3 currentDirection;

    float emissiveIntensity = 15555555;


    // Start is called before the first frame update
    void Start()
    {

        if (!celestialBody) Debug.Log("The Celestial Body Game Object is not assigned", this);
        //celestialBody.GetComponent.material.EnableKeyword("_EmissionColor");



    }

    // Update is called once per frame
    void Update()
    {
        DateTime currentTime = System.DateTime.Now;

        float currentSecTime = currentTime.Hour * 3600 + currentTime.Hour * 60 + currentTime.Second;
        currentSunT = Mathf.InverseLerp(StartHour * 3600 + StartMinute* 60, EndHour * 3600 + EndMinute * 60, currentSecTime);

        var Body = celestialBody.GetComponent<Renderer>();

        Body.material.SetColor("_EmissiveColor", sunStartColor* emissiveIntensity);
        //Body.material.SetColor("Emissive Intensity", emissiveIntensity);
        Body.material.SetColor("_Color", sunStartColor);
        

        if (timeElapsed < lerpDuration)
            currentDirection = Vector3.Lerp(startDirection, endDirection,timeElapsed/lerpDuration);
        if (DoDaySync)
        {
            currentDirection = Vector3.Lerp(startDirection, endDirection, currentSunT);
        }

        sun.transform.localEulerAngles = currentDirection;







        timeElapsed += Time.deltaTime;
    }
}
