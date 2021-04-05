using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBaseCode : MonoBehaviour
{
    [Header("Model")]
    public GameObject barrelModel;
    public BoxCollider barrelCollider;

    [Header("Audio")]
    public AudioEvent barrelAmbienceSFX;
    public AudioSource barrelAmbienceSource;
    public AudioEvent barrelCollisionSFX;
    public AudioSource barrelCollisionSource;

    [Header("DestructEvent")]
    public DestructEvent barrelDestructEvent;

    private void Start()
    {
        barrelAmbienceSFX.Play(barrelAmbienceSource);
    }

    public void BarrelCollisionSound()
    {
        barrelAmbienceSource.Stop();
        barrelCollisionSFX.Play(barrelCollisionSource);
    }

    public void DeleteBarrelModel()
    {
        barrelDestructEvent.Play(barrelModel);
    }

    public void DeleteBarrelCompletly()
    {
        Destroy(gameObject);
    }
}