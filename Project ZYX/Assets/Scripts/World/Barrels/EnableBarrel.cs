using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBarrel : MonoBehaviour
{
    [SerializeField] private GameObject Barrel;
    
    private void Awake()
    {
        Game.OnNewMatch += () => BarrelActivator();
    }

    private void BarrelActivator()
    {
        Barrel.gameObject.SetActive(true);
    }
}
