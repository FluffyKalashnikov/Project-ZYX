using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Spawn position offset")]
    [SerializeField] private float xposMin;
    [SerializeField] private float yposMin;
    [SerializeField] private float zposMin;
    [Space(20)]
    [SerializeField] private float xposMax;
    [SerializeField] private float yposMax;
    [SerializeField] private float zposMax;

    [Header("Small")] 
    [SerializeField] private GameObject fishSmall;
    [SerializeField] private int ammountSmall;
    [SerializeField] private float minWaitTimeSmall;
    [SerializeField] private float maxWaitTimeSmall;

    [Header("Medium")]
    [SerializeField] private GameObject fishMedium;
    [SerializeField] private int ammountMedium;
    [SerializeField] private float minWaitTimeMedium;
    [SerializeField] private float maxWaitTimeMedium;

    [Header("Big")]
    [SerializeField] private GameObject fishBig;
    [SerializeField] private int ammountBig;
    [SerializeField] private float minWaitTimeBig;
    [SerializeField] private float maxWaitTimeBig;
    // Start is called before the first frame update
    private void Awake()
    {
        Game.OnNewMatch += () => startco();
        Game.OnEndMatch += () => stopco();
    }
    void startco()
    {
        StartCoroutine(SpawnFishSmallCO());
        StartCoroutine(SpawnFishMediumCO());
        StartCoroutine(SpawnFishBigCO());
    }
    void stopco()
    {
        StopCoroutine(SpawnFishSmallCO());
        StopCoroutine(SpawnFishMediumCO());
        StopCoroutine(SpawnFishBigCO());
    }
    #region Small
    IEnumerator SpawnFishSmallCO()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTimeSmall, maxWaitTimeSmall));
            SpawnFishSmall();
        }
    }
    private void SpawnFishSmall()
    {

        for (int i = 0; i < ammountSmall; i++)
        {
            Vector3 randomizedPos = new Vector3(Random.Range(xposMin, xposMax), Random.Range(yposMin, yposMax), Random.Range(zposMin, zposMax));
            Instantiate(fishSmall, transform.position + randomizedPos, gameObject.transform.rotation);
        }
    }
    #endregion

    #region Medium
    IEnumerator SpawnFishMediumCO()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTimeMedium, maxWaitTimeMedium));
            SpawnFishMedium();
        }
    }
    private void SpawnFishMedium()
    {
        for (int i = 0; i < ammountMedium; i++)
        {
            Vector3 randomizedPos = new Vector3(Random.Range(xposMin, xposMax), Random.Range(yposMin, yposMax), Random.Range(zposMin, zposMax));
            Instantiate(fishMedium, transform.position + randomizedPos, gameObject.transform.rotation);
        }
    }
    #endregion
    
    #region Big
    IEnumerator SpawnFishBigCO()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTimeBig, maxWaitTimeBig));
            SpawnFishBig();
        }
    }
    private void SpawnFishBig()
    {
        for (int i = 0; i < ammountBig; i++)
        {
            Vector3 randomizedPos = new Vector3(Random.Range(xposMin, xposMax), Random.Range(yposMin, yposMax), Random.Range(zposMin, zposMax));
            Instantiate(fishBig, transform.position + randomizedPos, gameObject.transform.rotation);
        }
    }
    #endregion
}
