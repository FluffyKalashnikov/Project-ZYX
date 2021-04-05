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
    private void OnTriggerEnter(Collider seamine)
    {
        if (seamine.gameObject.layer == 8 || seamine.gameObject.layer == 9)
        {
            seamineCollider.enabled = false;
            SeamineCollisionSound();
            DeleteSeamineModel();
            //Some kind of tank damage code here and some fire animations aswell
            StartCoroutine(SeamineDestroyer());
        }
    }
    IEnumerator SeamineDestroyer()
    {
        yield return new WaitForSeconds(seamineCollisionSource.clip.length);
        DeleteSeamineCompletly();
    }

    public void SeamineCollisionSound()
    {
        seamineAmbienceSource.Stop();
        seamineCollisionSFX.Play(seamineCollisionSource);
    }

    public void DeleteSeamineModel()
    {
        seamineDestructEvent.Play(seamineModel);
    }

    public void DeleteSeamineCompletly()
    {
        Destroy(gameObject);
    }
}