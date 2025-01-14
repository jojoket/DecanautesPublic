using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.LookDev;

public class script_Day_Night : MonoBehaviour
{

    //public float colorA = 1;
    //public float colorB = 0; 

    public bool DoDaySync;
    [ShowIf("DoDaySync")]
    public script_Light_Day_Night dayNight;

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

        currentColorTop = startColorTop;
        currentColorMid = startColorMid;


        sky.top.value = startColorTop;
        sky.middle.value = startColorMid;
        sky.bottom.value = startColorBottom;




    }

    // Update is called once per frame
    void Update()
    {
          
        if (DoDaySync)
        {
            lerpDuration = 1;
            timeElapsed = dayNight.currentSunT;
        }
        

            

           if (timeElapsed < lerpDuration2 && timeElapsed > (lerpDuration * 0.6f))
            {
                currentColorTop = Color.Lerp(startColorTop, middleColorTop, (timeElapsed - lerpDuration*0.6f) / (lerpDuration * 0.1f));
                currentColorMid = Color.Lerp(startColorMid, middleColorMid, (timeElapsed - lerpDuration * 0.6f) / (lerpDuration * 0.1f));
                currentColorBottom = Color.Lerp(startColorBottom, middleColorBottom, (timeElapsed - lerpDuration * 0.6f) / (lerpDuration * 0.1f));
            }

            else if (timeElapsed > lerpDuration2)
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
