using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire", menuName = "ZYX Assets/Fire Event/SingleMulti")]
public class FireEventSingleMulti : FireEvent
{
    [Header("Multishot Settings")]
    
    [Tooltip("Ammount of bullets that will be fired when multishot is enabled")]
    public float Multishot_Ammount = 3;
    
    [Tooltip("The angle the bullets will fly in")]
    public float Multishot_Angle = 30;
    // Start is called before the first frame update

    public override void Fire()
    {

    }
}
