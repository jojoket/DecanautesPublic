using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class script_Day_Night : MonoBehaviour
{       

   //public float colorA = 1;
   //public float colorB = 0; 

    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]


    public Color startColorTop;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
    public Color startColorMid;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
    public Color startColorBottom;

    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
    public Color finalColorTop;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
    public Color finalColorMid;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
    public Color finalColorBottom;



    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color middleColorTop;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color middleColorMid;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color middleColorBottom;


    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
   Color currentColorTop;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
   Color currentColorMid;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)]
   Color currentColorBottom;

   public float lerpDuration = 10;
   float timeElapsed;
   float lerpDuration2;
    float lerpDuration3;

    GradientSky sky;
    

    // Start is called before the first frame update
    void Start()
    {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out sky);
        lerpDuration2 = lerpDuration * 0.7f;
        lerpDuration3 = lerpDuration * 0.3f;

    }

    // Update is called once per frame
    void Update()
    {
    
        if (timeElapsed < lerpDuration2)
        { 
            currentColorTop = Color.Lerp(startColorTop, middleColorTop, timeElapsed/ lerpDuration2);
            currentColorMid = Color.Lerp(startColorMid, middleColorMid, timeElapsed/ lerpDuration2);
            currentColorBottom = Color.Lerp(startColorBottom, middleColorBottom, timeElapsed/ lerpDuration2);
        }

        if (timeElapsed > lerpDuration2)
        {
            currentColorTop = Color.Lerp(middleColorTop, finalColorTop, (timeElapsed - lerpDuration2) / lerpDuration3);
            currentColorMid = Color.Lerp(middleColorMid, finalColorMid, (timeElapsed - lerpDuration2) / lerpDuration3);
            currentColorBottom = Color.Lerp(middleColorBottom, finalColorBottom, (timeElapsed - lerpDuration2) / lerpDuration3);

        }



        sky.top.value = currentColorTop;
        sky.middle.value = currentColorMid;
        sky.bottom.value = currentColorBottom;

         timeElapsed += Time.deltaTime;
        
    }
}
