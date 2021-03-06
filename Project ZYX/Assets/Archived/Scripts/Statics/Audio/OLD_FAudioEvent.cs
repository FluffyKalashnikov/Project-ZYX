using UnityEngine;

public abstract class OLD_FAudioEvent : ScriptableObject
{
    public AudioSource source = null;

    public abstract void Play(AudioSource source);
}


//[CreateAssetMenu(fileName = "AudioAsset", menuName = "FAudioEvent/SimpleAudio")]
public class SimpleAudio : OLD_FAudioEvent
{
    public AudioClip[]  clips  = null;
    public float        volume = 0.2f;
    public float        pitch  = 1f;


    public override void Play(AudioSource source)
    {
        source.clip   = clips[Random.Range(0, clips.Length)];
        source.volume = volume;
        source.pitch  = pitch;
        source.Play();
    }
}