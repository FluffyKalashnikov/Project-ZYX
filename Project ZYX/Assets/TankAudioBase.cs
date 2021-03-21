using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAudioBase : MonoBehaviour
{
    TankMovement tankMovement;

    [SerializeField] string name;
    public virtual void EnableAudio()
    {
        Debug.Log("enabledAudio");
        tankMovement = GetComponentInParent<TankMovement>();
    }
    public virtual void DisableAudio()
    {
        Debug.Log("disabledAudio");
    }
    public virtual void MoveTick()
    {

    }
}
