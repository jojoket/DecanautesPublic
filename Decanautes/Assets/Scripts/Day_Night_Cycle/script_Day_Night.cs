using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class script_Day_Night : MonoBehaviour
{       

   //public float colorA = 1;
   //public float colorB = 0; 

   public Color startColorTop;
   public Color startColorMid;
   public Color startColorBottom;

    public Color finalColorTop;
    public Color finalColorMid;
    public Color finalColorBottom;
   

   //float finalColor;

   Color currentColorTop;
   Color currentColorMid;
   Color currentColorBottom;

   public float lerpDuration = 10;
   float timeElapsed;

    GradientSky sky;
    

    // Start is called before the first frame update
    void Start()
    { Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out sky);
    } 

    // Update is called once per frame
    void Update()
    {   
        if (timeElapsed < lerpDuration)
        currentColorTop = Color.Lerp(startColorTop, finalColorTop, timeElapsed/lerpDuration);
        currentColorMid = Color.Lerp(startColorMid, finalColorMid, timeElapsed/lerpDuration);
        currentColorBottom = Color.Lerp(startColorBottom, finalColorBottom, timeElapsed/lerpDuration);



        sky.top.value = currentColorTop;
        sky.middle.value = currentColorMid;
        sky.bottom.value = currentColorBottom;

         timeElapsed += Time.deltaTime;
    }
}
