using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FireEvent : ScriptableObject
{
    public abstract void Fire(Tank TankScript, Transform MuzzlePoint, float charge, float bulletVelocity);
}
