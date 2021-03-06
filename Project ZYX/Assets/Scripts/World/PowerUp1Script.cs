using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp1Script : MonoBehaviour
{
    
   

   
    private void OnTriggerEnter(Collider collider)
    { 
        TankMovement tankmove = collider.GetComponentInParent<TankMovement>();
        tankmove.Timer = 10;
        Destroy(this.gameObject);
    }
}
