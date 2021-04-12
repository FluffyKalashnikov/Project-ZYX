using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire", menuName = "ZYX Assets/Fire Event/SingleMulti")]
public class FireEventSingleMulti : FireEvent
{
    [Header("Shell Prefab")]
    public GameObject ShellPrefab;

    [Header("Enable Multishot")]
    public bool Multishot_Enabled;

    [Header("Multishot Settings")]
    [Tooltip("Ammount of bullets that will be fired when multishot is enabled")]
    public float Multishot_Ammount = 3;
    
    [Tooltip("The angle the bullets will fly in")]
    public float Multishot_Angle = 30;
    // Start is called before the first frame update

    public override void Fire(Tank TankScript, Transform MuzzlePoint, float charge, float bulletVelocity)
    {
        if (Multishot_Enabled == true)
        {
            for (int i = 0; i < Multishot_Ammount; i++)
            {
                Quaternion target = Quaternion.AngleAxis(Multishot_Angle * (i - (Multishot_Ammount / 2)) - -1 * (Multishot_Angle / 2), MuzzlePoint.up);
                // 1. CREATE BULLET
                var Shell = Instantiate
                (
                    ShellPrefab,
                    MuzzlePoint.position,
                    target * MuzzlePoint.rotation
                ).GetComponent<Shell>();

                // 2. INIT BULLET
                Shell.Init(bulletVelocity * charge, TankScript);
                Tank.OnTankFire.Invoke(TankScript);
            }
        }

        else if (Multishot_Enabled == false)
        {
            // 1. CREATE BULLET
            var Shell = Instantiate
            (
                ShellPrefab,
                MuzzlePoint.position,
                MuzzlePoint.rotation
            ).GetComponent<Shell>();

            // 2. INIT BULLET
            Shell.Init(bulletVelocity * charge, TankScript);
            Tank.OnTankFire.Invoke(TankScript);
        }
    }
}
