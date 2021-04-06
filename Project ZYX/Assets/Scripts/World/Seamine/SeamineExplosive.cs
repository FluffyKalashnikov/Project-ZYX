using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamineExplosive : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private GameObject seamineModel;
    [SerializeField] private BoxCollider seamineCollider;

    [Header("Audio")]
    [SerializeField] private AudioEvent seamineSonarAmbienceSFX;
    [SerializeField] private AudioSource seamineSonarAmbienceSource;
    [SerializeField] private AudioEvent seamineSwingAmbienceSFX;
    [SerializeField] private AudioSource seamineSwingAmbienceSource;
    [SerializeField] private AudioEvent seamineCollisionSFX;
    [SerializeField] private AudioSource seamineCollisionSource;

    [Header("DestructEvent")]
    public DestructEvent seamineDestructEvent;

    private bool destroyed = false;

    private void Start()
    {
        seamineSonarAmbienceSFX.Play(seamineSonarAmbienceSource);
    }

    private void Update()
    {
        SwingSound();
    }
    private void SwingSound()
    {
        if(seamineSwingAmbienceSource.isPlaying == false && !destroyed)
        {
            seamineSwingAmbienceSFX.Play(seamineSwingAmbienceSource);
        }
        else if (destroyed)
        {
            seamineSwingAmbienceSource.Stop();
        }
        else
        {
            return;
        }
    }
    private void OnTriggerEnter(Collider seamine)
    {
        if (seamine.gameObject.layer == 8 || seamine.gameObject.layer == 9)
        {
            seamineCollider.enabled = false;
            destroyed = true;
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
        seamineSonarAmbienceSource.Stop();
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