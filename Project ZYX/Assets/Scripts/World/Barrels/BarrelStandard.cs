using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelStandard : BarrelBaseCode
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.tag == "Bullet")
        {
            barrelCollider.enabled = false;
            BarrelCollisionSound();
            DeleteBarrelModel();
            //Some kind of particle animation
            StartCoroutine(BarrelDestroyer());
        }
    }
    IEnumerator BarrelDestroyer()
    {
        yield return new WaitForSeconds(barrelCollisionSource.clip.length);
        DeleteBarrelCompletly();
    }
}