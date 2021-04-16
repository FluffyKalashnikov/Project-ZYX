using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BarrelRadioactive : BarrelBaseCode
{
    [SerializeField] private RadiationTimerFunction radTimer;
    [SerializeField] private VisualEffect explosionFX1;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            explosionFX1.Play();
            radTimer.ActivateRadioation(collision);
        }
    }
}