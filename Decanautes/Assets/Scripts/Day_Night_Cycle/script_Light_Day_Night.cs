using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Experimental.GlobalIllumination;

public class script_Light_Day_Night : MonoBehaviour
{

    
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
        var Body = celestialBody.GetComponent<Renderer>();

        Body.material.SetColor("_EmissiveColor", sunStartColor* emissiveIntensity);
        //Body.material.SetColor("Emissive Intensity", emissiveIntensity);
        Body.material.SetColor("_Color", sunStartColor);
        

        if (timeElapsed < lerpDuration)
            currentDirection = Vector3.Lerp(startDirection, endDirection,timeElapsed/lerpDuration);


        sun.transform.localEulerAngles = currentDirection;







        timeElapsed += Time.deltaTime;
    }
}
