using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationTimerFunction : MonoBehaviour
{
    private Tank owner = null;
    [SerializeField] private BarrelBaseCode barrelBase;

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
    #region Timer Reset
    private void Awake()
    {
        Game.OnNewLobby += () => ResetStats();
        Game.OnNewMatch += () => ResetTimer();

        Game.OnTankSpawn += (tank) => ResetEverything();
    }
    private void ResetStats()
    {
        StopCoroutine(RadioactiveTimer());
        hitRadioactiveBarrel = false;
        StopCoroutine(RadioactiveDamage());

        currentRadOverallDuration = radOverallDuration;
    }
    private void ResetTimer()
    {
        currentRadOverallDuration = radOverallDuration;
    }
    private void ResetEverything()
    {
        StopCoroutine(RadioactiveTimer());
        hitRadioactiveBarrel = false;
        StopCoroutine(RadioactiveDamage());

        currentRadOverallDuration = radOverallDuration;
    }
    #endregion
    public void ActivateRadioation(Collider collision)
    {
        #region General
        barrelBase.barrelCollider.enabled = false;
        barrelBase.BarrelCollisionSound();
        barrelBase.DeleteBarrelModel();
        //Some kind of tank damage code here and some radiation animations aswell?
        #endregion

        radCol = collision;
        hitRadioactiveBarrel = true;
    }
    
    #region Radioation Function
    private void Update()
    {
        if (hitRadioactiveBarrel == true)
        {
            if (radiationPulse == false && currentRadOverallDuration > 0)
            {
                MatchContext.Add(RadioactiveDamage());
            }
            else if (radiationTimer == false && currentRadOverallDuration > 0)
            {
                MatchContext.Add(RadioactiveTimer());
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
            if (radCol != null)
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
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator RadioactiveTimer()
    {
        radiationTimer = true;
        yield return new WaitForSeconds(1);
        currentRadOverallDuration -= 1;
        //Set bool to false
        radiationTimer = false;
        
        if(currentRadOverallDuration == 0)
        {
            barrelBase.DeleteBarrelCompletly();
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /*public IEnumerator BarrelDestroyer()
    {
        yield return new WaitForSeconds(currentRadOverallDuration);

    }*/
    #endregion
}
