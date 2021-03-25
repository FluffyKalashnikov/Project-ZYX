using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float TakeDamage(float damage, DamageInfo info, MonoBehaviour dealer);
}

public class DamageInfo
{
    public DamageType type;

    public DamageInfo(DamageType type)
    {
        this.type = type;
    }
}

public enum DamageType
{
    ShellImpact,
    ShellExplosion
}



