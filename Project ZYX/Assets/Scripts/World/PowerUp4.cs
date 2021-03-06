using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp4 : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        TankShoot tank = collider.GetComponentInParent<TankShoot>();
        tank.timer = 10;
        Destroy(this.gameObject);
    }
}
