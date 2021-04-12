using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRadioactive : BarrelBaseCode
{
    private Tank owner = null;

    [Header("Radiation Values")]
    [SerializeField] private float radiationDamage;
    [Tooltip("The radiation works as a poision potion in Minecraft, for every set second, you lose a life (Instead of having a gradual loss)")]
    [SerializeField] private float radPulseDuration;
    [Tooltip("The overall duration is the duration of the radiation process (ONLY WORKS WITH INTS)")]
    [SerializeField] private int radOverallDuration;

    private Collider radCol;
    private float currentRadOverallDuration;
    private bool hitRadioactiveBarrel = false;
    private bool radiationPulse = false;
    private bool radiationTimer = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            radCol = collision;

            currentRadOverallDuration = radOverallDuration;
            hitRadioactiveBarrel = true;

            #region General
            barrelCollider.enabled = false;
            BarrelCollisionSound();
            DeleteBarrelModel();
            //Some kind of tank damage code here and some radiation animations aswell?
            StartCoroutine(BarrelDestroyer());
            #endregion
        }
    }
    #region Radioation Function
    private void Update()
    {
        if (hitRadioactiveBarrel == true)
        {
            if (radiationPulse == false && currentRadOverallDuration > 0)
            {
                StartCoroutine(RadioactiveDamage());
            }
            else if (radiationTimer == false && currentRadOverallDuration > 0)
            {
                StartCoroutine(RadioactiveTimer());
            }
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator RadioactiveDamage()
    {
        radiationPulse = true;
        yield return new WaitForSeconds(radPulseDuration);

        if (currentRadOverallDuration != 0)
        {
            if(radCol != null)
            {
                IDamageable hit = radCol.gameObject.GetComponentInParent<IDamageable>();

                hit?.TakeDamage
                (
                    new DamageInfo
                    (
                        radiationDamage,
                        DamageType.RadioactiveBarrel,
                        owner,
                        hit
                    )
                );
            }
        }
        //Set bool to false
        radiationPulse = false;
    }
    IEnumerator RadioactiveTimer()
    {
        radiationTimer = true;
        yield return new WaitForSeconds(1);
        currentRadOverallDuration -= 1;

        //Set bool to false
        radiationTimer = false;
    }
    #endregion
    IEnumerator BarrelDestroyer()
    {
        yield return new WaitForSeconds(currentRadOverallDuration);
        DeleteBarrelCompletly();
    }
}