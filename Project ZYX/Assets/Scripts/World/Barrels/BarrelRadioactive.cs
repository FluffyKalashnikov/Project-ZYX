using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRadioactive : BarrelBaseCode
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.tag == "Bullet")
        {
            barrelCollider.enabled = false;
            BarrelCollisionSound();
            DeleteBarrelModel();
            //Some kind of tank damage code here and some radiation animations aswell?
            StartCoroutine(BarrelDestroyer());
        }
    }
    IEnumerator BarrelDestroyer()
    {
        yield return new WaitForSeconds(barrelCollisionSource.clip.length);
        DeleteBarrelCompletly();
    }
}