using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="KilometerData", menuName ="ScriptableObject/KilometerData")]
public class KilometerData : ScriptableObject
{
    public enum SpeedType
    {
        Normal,
        Overlock,
        Malfunction,
        Breakdown,
    }

    public float CurrentKm;
    public float CurrentSpeed;
    public float NormalSpeed;
    public float OverlockSpeed;
    public float MalfunctionSpeed;
    public float BreakdownSpeed;

    public SpeedType speedType;
}
