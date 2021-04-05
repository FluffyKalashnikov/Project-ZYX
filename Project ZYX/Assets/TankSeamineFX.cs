using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSeamineFX : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private Tank tankScript;

    [Header("Seamine Damage")]
    [SerializeField] private float mineDamage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SeamineDamageable")
        {
            tankScript.Health -= mineDamage;
        }
    }
}
