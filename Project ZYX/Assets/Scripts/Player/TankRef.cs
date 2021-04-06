using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRef : MonoBehaviour
{
    public Vector3 ModelOffset = Vector3.zero;
    [Space(5)]
    public Transform MuzzlePoint     = null;
    public Transform TurretTransform = null;
    public Transform SeaminePoint = null;
}
