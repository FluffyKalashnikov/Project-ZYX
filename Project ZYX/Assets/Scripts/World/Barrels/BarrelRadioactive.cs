using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRadioactive : BarrelBaseCode
{
    [SerializeField] private RadiationTimerFunction radTimer;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            radTimer.ActivateRadioation(collision);
        }
    }
}