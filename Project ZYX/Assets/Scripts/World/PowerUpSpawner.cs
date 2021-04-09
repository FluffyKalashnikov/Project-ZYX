using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Pickup respawn time")]
    [SerializeField] private int respawnTime;

    [Header("Pickup Spawning Variables")]

    [Tooltip("If unselected, then you're able to control what pickup the barrel should spawn. If selected, then the game randomly selects a value for you")]
    [SerializeField] private bool RandomizePickups;

    [Space(10)]

    [SerializeField] private int selectedPickup;
    [SerializeField] private List<GameObject> PowerupList = new List<GameObject>();

    [Space(10)]

    [SerializeField] private LayerMask layerMask;

    private int randomizedPickup;
    private int currentRespawnTimerValue;
    private bool spawned;
    private bool spawnerTimerBool;
    private bool addtotimer = false;

    private void Update()
    {
        #region RNG
        if (RandomizePickups)
        {
            randomizedPickup = Random.Range(0, PowerupList.Count);
        }
        else
        {
            return;
        }
        #endregion

        Debug.Log(currentRespawnTimerValue);

        RaycastHit hit;
        if (Physics.Raycast(transform.position - new Vector3(0, 1, 0), transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
        {
            addtotimer = false;
            spawned = true;
        }

        else
        {
            PowerupSpawnerTimerMethod();
            addtotimer = true;

            if (addtotimer == true)
            {
                currentRespawnTimerValue = respawnTime;
                addtotimer = false;
            }
        }
    }

    private void Start()
    {
        spawned = true;
        addtotimer = false;

        if (RandomizePickups)
        {
            Instantiate(PowerupList[randomizedPickup], gameObject.transform.position, Quaternion.identity);
            spawned = true;
        }
        else
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
        else
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
