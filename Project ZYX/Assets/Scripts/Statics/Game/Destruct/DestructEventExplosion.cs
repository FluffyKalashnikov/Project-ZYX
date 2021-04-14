using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "WorkingExplosion", menuName = "ZYX Assets/Destruct Event/ExplosionParticle")]
public class DestructEventExplosion : DestructEvent
{
    [SerializeField] private VisualEffect explosionFX1;
    [SerializeField] private VisualEffect explosionFX2;
    public override void Play(GameObject gameObject)
    {
        gameObject = null;
        explosionFX1.Play();
        explosionFX2.Play();

        throw new System.NotImplementedException();

    }
}
