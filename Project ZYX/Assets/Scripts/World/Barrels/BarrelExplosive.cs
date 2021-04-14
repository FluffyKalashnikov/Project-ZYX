using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BarrelExplosive : BarrelBaseCode
{
    private Tank owner = null;

    [Header("Barrel Damage")]
    [SerializeField] private float explosionDamage;
    [SerializeField] private VisualEffect explosionFX1;
    [SerializeField] private VisualEffect explosionFX2;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            #region DamageDealer
            IDamageable hit = collision.gameObject.GetComponentInParent<IDamageable>();

            hit?.TakeDamage
            (
                new DamageInfo
                (
                    explosionDamage,
                    DamageType.ExplosiveBarrel,
                    owner,
                    hit
                )
            );
            #endregion

            explosionFX1.Play();
            explosionFX2.Play();

            barrelCollider.enabled = false;
            BarrelCollisionSound();
            DeleteBarrelModel();
            //Some kind of tank damage code here and some fire animations aswell
            StartCoroutine(BarrelDestroyer());
        }
    }
    IEnumerator BarrelDestroyer()
    {
        yield return new WaitForSeconds(barrelCollisionSource.clip.length);
        DeleteBarrelCompletly();
    }
}