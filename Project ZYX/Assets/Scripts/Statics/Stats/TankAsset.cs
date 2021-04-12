using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tank Type", menuName = "ZYX Assets/Tank Type/Standard")]
public class TankAsset : ScriptableObject
{
    public string     Name   = "TANK NAME";
    public GameObject Model  = null;
    [Space(10)]
    public float Damage = 1f;
    public float Health = 100f;
    public float Speed  = 5f;
    public float VelocityMax = 5f;
    [Space(10)]
    public AudioEvent AudioIdle     = null;
    public AudioEvent AudioStartup  = null;
    public AudioEvent AudioThrottle = null;
    public AudioEvent AudioRevLow = null;
    public AudioEvent AudioRevMid = null;
    public AudioEvent AudioRevHigh = null;
    public AudioEvent ChargeAbility = null;
    [Space(10)]
    public FireEventSingleMulti FireChargeAbility;
}
