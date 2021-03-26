﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRadioactive : BarrelBaseCode
{
    private void OnCollisionEnter(Collision collision)
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.tag == "Bullet")
        {
            BarrelCollisionSound();
            DeleteBarrelModel();
            //Some kind of tank damage code here and some radiation animations aswell?
            if (barrelCollisionSource.isPlaying == false)
            {
                DeleteBarrelCompletly();
            }
        }
    }
}
