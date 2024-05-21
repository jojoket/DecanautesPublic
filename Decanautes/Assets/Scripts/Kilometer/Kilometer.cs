using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Kilometer : MonoBehaviour
{

    public KilometerData data;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        data.CurrentSpeed = 0;
        switch (data.speedType)
        {
            case KilometerData.SpeedType.Normal:
                data.CurrentSpeed = data.NormalSpeed;
                break;
            case KilometerData.SpeedType.Overlock:
                data.CurrentSpeed = data.OverlockSpeed;
                break;
            case KilometerData.SpeedType.Malfunction:
                data.CurrentSpeed = data.MalfunctionSpeed;
                break;
            case KilometerData.SpeedType.Breakdown:
                data.CurrentSpeed = data.BreakdownSpeed;
                break;
            default:
                break;
        }

        data.CurrentKm += data.CurrentSpeed * Time.deltaTime;
    }
}
