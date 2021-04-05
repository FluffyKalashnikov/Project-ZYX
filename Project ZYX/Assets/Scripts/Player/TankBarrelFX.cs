using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBarrelFX : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private Tank tankScript;

    [Header("Explosive")]
    [SerializeField] private float explDamage;

    [Header("Radioactive")]
    [SerializeField] private float radDamage;
    [Tooltip("The radiation works as a poision potion in Minecraft, for every set second, you lose a life (Instead of having a gradual loss)")]
    [SerializeField] private float radPulseDuration;
    [Tooltip("The overall duration is the duration of the radiation process (ONLY WORKS WITH INTS)")]
    [SerializeField] private int radOverallDuration;

    private float currentRadOverallDuration;
    private bool hitRadioactiveBarrel = false;
    private bool radiationPulse = false;
    private bool radiationTimer = false;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "BarrelExplosive":
                tankScript.Health -= explDamage;
                break;
            case "BarrelRadioactive":
                currentRadOverallDuration = radOverallDuration;
                hitRadioactiveBarrel = true;
                break;
        }
    }
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
            Debug.Log(currentRadOverallDuration == 0);
            Debug.Log(currentRadOverallDuration);
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator RadioactiveDamage()
    {
        radiationPulse = true;
        yield return new WaitForSeconds(radPulseDuration);
        
        if(currentRadOverallDuration !=0)
        {
            tankScript.Health -= radDamage;
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
}
