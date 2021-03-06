using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    Tank tank;
    // Start is called before the first frame update
    void Start()
    {
        Tank tank = gameObject.GetComponent<Tank>();
    }

   
    private void OnCollisionEnter(Collision collider)
    {
        tank.Power = 1;
        tank.PowerUpTimer = 10;
        Destroy(this.gameObject);
    }
}
