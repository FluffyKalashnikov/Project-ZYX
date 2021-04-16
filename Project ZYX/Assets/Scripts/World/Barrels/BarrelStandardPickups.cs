using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BarrelStandardPickups : BarrelBaseCode
{
    /*[Header("Pickup Spawning Type")]
    [Tooltip("If unselected, then you're able to control what pickup the barrel should spawn. If selected, then the game randomly selects a value for you")]
    [SerializeField] private bool RandomizePickups;
    
    [Space(10)]
    
    [Tooltip("Check the elements numbers!")]
    [SerializeField] private int selectedPickup;*/
    [SerializeField] private VisualEffect explosionFX1;

    [Header("Pickup List")]
    [SerializeField] private List<GameObject> PowerupList = new List<GameObject>();
    
    private int randomizedPickup;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            explosionFX1.Play();
            barrelCollider.enabled = false;
            BarrelCollisionSound();
            DeleteBarrelModel();
            SpawnPickup();
            //Some kind of particle animation
            StartCoroutine(BarrelDestroyer());
        }
    }
    private void Update()
    {
        randomizedPickup = Random.Range(0, PowerupList.Count);
    }
    private void SpawnPickup()
    {
         Instantiate(PowerupList[randomizedPickup], gameObject.transform.position, Quaternion.identity);
    }
    IEnumerator BarrelDestroyer()
    {
        yield return new WaitForSeconds(barrelCollisionSource.clip.length);
        DeleteBarrelCompletly();
    }
}