using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBaseCode : MonoBehaviour
{
    [Header("Model")]
    public GameObject barrelModel;

    [Header("Audio")]
    public AudioEvent barrelAmbienceSFX;
    public AudioSource barrelAmbienceSource;
    public AudioEvent barrelCollisionSFX;
    public AudioSource barrelCollisionSource;

    private void Start()
    {
        barrelAmbienceSFX.Play(barrelAmbienceSource);
    }

    public void BarrelCollisionSound()
    {
        barrelAmbienceSource.Stop();
        barrelCollisionSFX.Play(barrelCollisionSource);
        Debug.Log("collision sound");
    }

    public void DeleteBarrelModel()
    {
        Destroy(barrelModel.gameObject);
    }

    public void DeleteBarrelCompletly()
    {
        Destroy(gameObject);
    }
}
