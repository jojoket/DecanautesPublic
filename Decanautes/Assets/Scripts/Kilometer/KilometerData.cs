using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Unit(Units.KilometersPerHour)]
    public float CurrentSpeed;
    [Unit(Units.KilometersPerHour)]
    public float NormalSpeed;
    [Unit(Units.KilometersPerHour)]
    public float OverlockSpeed;
    [Unit(Units.KilometersPerHour)]
    public float MalfunctionSpeed;
    [Unit(Units.KilometersPerHour)]
    public float BreakdownSpeed;

    public SpeedType speedType;
}
