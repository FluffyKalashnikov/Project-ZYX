using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelStandard : BarrelBaseCode
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bullet")
        {
            BarrelCollisionSound();
            //DeleteBarrelModel();
            //Some kind of particle animation
            /*if (barrelCollisionSource.isPlaying == false)
            {
                DeleteBarrelCompletly();
            }*/
        }
    }
}