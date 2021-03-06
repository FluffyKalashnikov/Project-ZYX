using System.Collections;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "ZYX Assets/Audio/Default")]
public class AudioEventDefault : AudioEvent
{
    [Header("Event Settings")]
    public AudioClip[] clips       = { null };
    public bool        loop        = false;
    public bool        playOneShot = false;
    [Space(5, order = 0)]
    [Header("Clip Properties", order = 1)]

    public RandFloat volume        =  new RandFloat(1f, 0f, 1f);
    public RandFloat pitch         =  new RandFloat(1f, -3f, 3f);
    public RandFloat spatialBlend  =  new RandFloat(0f, 0f, 1f);



    public override void Play(AudioSource source)
    {
        if (clips.Length == 0)
        {
            Debug.LogError($"AudioEvent \"{name}\" has no clips assigned.");
            return;
        }

        source.clip         = clips[Random.Range(0, clips.Length)];
        source.loop         = loop;
        source.volume       = volume      .Get();
        source.pitch        = pitch       .Get();
        source.spatialBlend = spatialBlend.Get();


        if (!source.clip)
        {
            Debug.LogError($"AudioEvent \"{name}\":s current clip is null.");
            return;
        }
 

        switch (playOneShot)
        {
            case true:  source.PlayOneShot(source.clip); break;
            case false: source.Play(); break;
        }
    }
}
