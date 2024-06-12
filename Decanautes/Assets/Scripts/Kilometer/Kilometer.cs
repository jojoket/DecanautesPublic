using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Kilometer : MonoBehaviour
{

    public KilometerData data;

    public List<Animator> animators;

    private float lastKilometer=0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (data.speedType)
        {
            case KilometerData.SpeedType.Normal:
                data.CurrentSpeed = data.NormalSpeed;
                break;
            case KilometerData.SpeedType.Overlock:
                data.CurrentSpeed = data.OverlockSpeed;
                break;
            case KilometerData.SpeedType.Malfunction:
                //Speed handled by EngineState script
                break;
            case KilometerData.SpeedType.Breakdown:
                data.CurrentSpeed = data.BreakdownSpeed;
                break;
            default:
                break;
        }

        data.CurrentKm += data.CurrentSpeed * Time.deltaTime;
        if (Mathf.RoundToInt(data.CurrentKm) != Mathf.RoundToInt(lastKilometer))
        {
            animators[0].SetFloat("TurnSpeed", data.CurrentSpeed);
            animators[0].SetTrigger("Turn");
        }

        if (Mathf.RoundToInt(data.CurrentKm/10) != Mathf.RoundToInt(lastKilometer/10))
        {
            animators[1].SetFloat("TurnSpeed", data.CurrentSpeed);
            animators[1].SetTrigger("Turn");
        }

        lastKilometer = data.CurrentKm;
    }
}
