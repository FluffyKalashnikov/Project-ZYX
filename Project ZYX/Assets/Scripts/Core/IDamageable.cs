using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(DamageInfo info);
}

public class DamageInfo
{
    // VARIABLES
    public float         Damage;
    public DamageType    DamageType;
    public MonoBehaviour Dealer;
    public IDamageable   Reciever;

    public DamageInfo(float Damage, DamageType DamageType, MonoBehaviour Dealer, IDamageable Reciever)
    {
        this.Damage     = Damage;
        this.DamageType = DamageType;
        this.Dealer     = Dealer;
        this.Reciever   = Reciever;
    }
}

public enum DamageType
{
    ShellImpact,
    ShellExplosion,
    SeamineImpact,
    ExplosiveBarrel,
    RadioactiveBarrel,
    OutOfBounds
}



