using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DestructEvent : ScriptableObject
{
    public abstract void Play(GameObject gameObject);
}
