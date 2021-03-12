using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSFX : MonoBehaviour
{
    [Header("Operation Aquatic (BATTLE THEME)")]
    [SerializeField] private AudioSource battleTrackSource;
    [SerializeField] private AudioEvent battleTrack;

    [Header("Played on start")]
    [SerializeField] private AudioEvent[] backgroundAmbientSound;

    [Header("Randomly played (WHALE)")]
    [SerializeField] private AudioEvent AmbientSoundsWHALE;
    [SerializeField] private AudioSource AmbientSourceWHALE;
    [SerializeField] private float minWaitTimeWHALE;
    [SerializeField] private float maxWaitTimeWHALE;

    [Header("Randomly played (BUBBLES)")]
    [SerializeField] private AudioEvent ambientSoundsBUBBLES;
    [SerializeField] private AudioSource whaleAmbientSourceBUBBLES;
    [SerializeField] private float minWaitTimeBUBBLES;
    [SerializeField] private float maxWaitTimeBUBBLES;

    [Header("Randomly played (DARK)")]
    [SerializeField] private AudioEvent ambientSoundsDARK;
    [SerializeField] private AudioSource whaleAmbientSourceDARK;
    [SerializeField] private float minWaitTimeDARK;
    [SerializeField] private float maxWaitTimeDARK;

    [Header("Randomly played (MISC)")]
    [SerializeField] private AudioEvent ambientSoundsMISC;
    [SerializeField] private AudioSource whaleAmbientSourceMISC;
    [SerializeField] private float minWaitTimeMISC;
    [SerializeField] private float maxWaitTimeMISC;

    private void Awake()
    {
        #region Starts background SFX
    List<AudioSource> audioSources = new List<AudioSource>();
    foreach (var i in backgroundAmbientSound) audioSources.Add(gameObject.AddComponent<AudioSource>());

    for (int i = 0; i < backgroundAmbientSound.Length; i++)
    {
        backgroundAmbientSound[i].Play(audioSources[i]);
    }
    #endregion
    }
    private void Start()
    {
        //PlayBattleTheme();

    StartCoroutine(WhaleSFX());
    StartCoroutine(BubblesSFX());
    StartCoroutine(DarkSFX());
    StartCoroutine(MiscSFX());
    }
    private void PlayBattleTheme()
    {
        battleTrack.Play(battleTrackSource);
    }


    IEnumerator WhaleSFX()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTimeWHALE, maxWaitTimeWHALE));
            PlayWhaleSFX();
        }
    }
    private void PlayWhaleSFX()
    {
        AmbientSoundsWHALE.Play(AmbientSourceWHALE);
    }

    IEnumerator BubblesSFX()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTimeBUBBLES, maxWaitTimeBUBBLES));
            PlayBubblesSFX();
        }
    }
    private void PlayBubblesSFX()
    {
        ambientSoundsBUBBLES.Play(whaleAmbientSourceBUBBLES);
    }

    IEnumerator DarkSFX()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTimeDARK, maxWaitTimeDARK));
            PlayDarkSFX();
        }
    }
    private void PlayDarkSFX()
    {
        ambientSoundsDARK.Play(whaleAmbientSourceDARK);
    }

    IEnumerator MiscSFX()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTimeMISC, maxWaitTimeMISC));
            PlayMiscSFX();
        }
    }
    private void PlayMiscSFX()
    {
        ambientSoundsMISC.Play(whaleAmbientSourceMISC);
    }
}
