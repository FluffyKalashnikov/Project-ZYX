using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleExplosion", menuName = "ZYX Assets/Destruct Event/SimpleExplosion")]
public class DestructEventSimpleDestroy : DestructEvent
{
    public override void Play(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
