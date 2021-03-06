using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp2Script : MonoBehaviour
{
    Tank tank;
    // Start is called before the first frame update
    void Start()
    {
        Tank tank = gameObject.GetComponent<Tank>();
    }


    private void OnTriggerEnter(Collider collider)
    {
        Tank tank = collider.GetComponentInParent<Tank>();
        tank.Power = 1;
        tank.PowerUpTimer = 10;
        Destroy(this.gameObject);
    }
    
  
}
