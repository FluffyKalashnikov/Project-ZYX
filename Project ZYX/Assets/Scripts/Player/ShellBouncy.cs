using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBouncy : Shell
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private int MaxRicochet;
    private int currentRicochet;
    private bool BouncyActivated;
    public override void CollisionAction(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            base.CollisionAction(collision);
        }
        else if(collision.gameObject.layer == 10)
        {
            if (currentRicochet == 0 && !BouncyActivated)
            {
                currentRicochet = MaxRicochet;
                BouncyActivated = true;
            }
            
            else if (currentRicochet != 0 && BouncyActivated)
            {
                Vector3 v = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
                float rot = 90 - Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, rot, 0);

                currentRicochet -= 1;
            }
            else if (currentRicochet == 0 && BouncyActivated)
            {
                DisableBullet();
                BouncyActivated = false;
            }
        }
    }
}
