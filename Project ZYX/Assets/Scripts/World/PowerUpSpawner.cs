using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Pickup respawn time")]
    [SerializeField] private int respawnTime;

    [Header("Pickup Spawning Type")]
    [Tooltip("If unselected, you're able to control what pickup the barrel should spawn. If selected, the game randomly selects a value for you. The value (SelectedPickup) does get used on Start though where the first ever pickup that spawns is the value you input here")]
    [SerializeField] private bool RandomizePickups;

    [Space(10)]

    [Tooltip("Check the elements numbers!")]
    [SerializeField] private int selectedPickup;

    [Header("Pickup List")]
    [SerializeField] private List<GameObject> PowerupList = new List<GameObject>();

    [Header("Other (DON'T TOUCH)")]
    [SerializeField] private LayerMask layerMask;

    private int randomizedPickup;
    private int currentRespawnTimerValue;
    private bool spawned;
    private bool spawnerTimerBool;

    private void Update()
    {
        #region RNG
        if (RandomizePickups)
        {
            randomizedPickup = Random.Range(0, PowerupList.Count);
        }
        #endregion

        RaycastHit hit;
        if (Physics.Raycast(transform.position - new Vector3(0, 1, 0), transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            spawned = true;
        }

        else
        {
            if (spawned && currentRespawnTimerValue == 0)
            {
                currentRespawnTimerValue = respawnTime;
            }
            PowerupSpawnerTimerMethod();
        }
    }

    private void Start()
    {
        spawned = true;

        if (RandomizePickups)
        {
            Instantiate(PowerupList[randomizedPickup], gameObject.transform.position, Quaternion.identity);
            spawned = true;
        }
        else if (!RandomizePickups)
        {
            Instantiate(PowerupList[selectedPickup], gameObject.transform.position, Quaternion.identity);
            spawned = true;
        }
    }
    private void PowerupSpawnerTimerMethod()
    {
        spawned = false;
        if(spawned == false)
        {
            if (spawnerTimerBool == false && currentRespawnTimerValue > 0)
            {
                StartCoroutine(PowerupRespawner());
            }
            else if (currentRespawnTimerValue == 0)
            {
                SpawnPickup();
                spawned = true;
            }
        }
    }
    private void SpawnPickup()
    {
        if (RandomizePickups)
        {
            Instantiate(PowerupList[randomizedPickup], gameObject.transform.position, Quaternion.identity);
        }
        else if (!RandomizePickups)
        {
            Instantiate(PowerupList[selectedPickup], gameObject.transform.position, Quaternion.identity);
        }
    }
    IEnumerator PowerupRespawner()
    {
        spawnerTimerBool = true;
        yield return new WaitForSeconds(1);
        currentRespawnTimerValue -= 1;

        //Set bool to false
        spawnerTimerBool = false;
    }
}
