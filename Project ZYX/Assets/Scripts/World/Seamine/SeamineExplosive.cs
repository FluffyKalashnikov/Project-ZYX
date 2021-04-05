using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamineExplosive : MonoBehaviour
{
    [Header("Model")]
    public GameObject seamineModel;
    public BoxCollider seamineCollider;

    [Header("Audio")]
    public AudioEvent seamineAmbienceSFX;
    public AudioSource seamineAmbienceSource;
    public AudioEvent seamineCollisionSFX;
    public AudioSource seamineCollisionSource;

    [Header("DestructEvent")]
    public DestructEvent seamineDestructEvent;

    private void Start()
    {
        //seamineAmbienceSFX.Play(seamineAmbienceSource);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            seamineCollider.enabled = false;
            BarrelCollisionSound();
            DeleteBarrelModel();
            //Some kind of tank damage code here and some fire animations aswell
            StartCoroutine(BarrelDestroyer());
        }
    }
    IEnumerator BarrelDestroyer()
    {
        yield return new WaitForSeconds(seamineCollisionSource.clip.length);
        DeleteBarrelCompletly();
    }

    public void BarrelCollisionSound()
    {
        seamineAmbienceSource.Stop();
        seamineCollisionSFX.Play(seamineCollisionSource);
    }

    public void DeleteBarrelModel()
    {
        seamineDestructEvent.Play(seamineModel);
    }

    public void DeleteBarrelCompletly()
    {
        Destroy(gameObject);
    }
}