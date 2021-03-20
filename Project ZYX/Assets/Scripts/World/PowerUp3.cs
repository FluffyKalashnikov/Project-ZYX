using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp3 : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        Tank tank = collider.GetComponentInParent<Tank>();
        tank.Health += 50;
        Destroy(this.gameObject);
    }
}
