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

    //[Header("DestructEvent")]
    //public DestructEvent barrelDestructEvent;
    private void Awake()
    {
        Game.OnNewLobby += () => BarrelActivator();
    }
    private void BarrelActivator()
    {
        barrelModel.gameObject.SetActive(true);
        barrelCollider.enabled = true;
    }
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
        barrelModel.SetActive(false);
    }

    public void DeleteBarrelCompletly()
    {
        gameObject.SetActive(false);
    }
}