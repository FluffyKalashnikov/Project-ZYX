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
    [Space(10)]
    public AudioClip AudioIdle     = null;
    public AudioClip AudioStartup  = null;
    public AudioClip AudioThrottle = null;
}
