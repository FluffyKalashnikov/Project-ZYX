using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDeathVFX : MonoBehaviour
{
    [SerializeField] private Tank tankScript;
    [SerializeField] private ParticleSystem engineSmoke;

    private bool isplaying = false;

    private void Awake()
    {
        Game.OnNewLobby += () => { stopsmoke(); };

        Game.OnNewMatch += () => { stopsmoke(); };
        Game.OnEndMatch += () => { stopsmoke(); };

        Game.OnTankSpawn += (tank) => { stopsmoke(); };
        Tank.OnDead += (tank) => { stopsmoke(); };
    }
    private void Update()
    {
        if(tankScript.HealthFactor <= 0.3f && tankScript.HealthFactor > 0 && isplaying == false)
        {
            engineSmoke.Play();
            isplaying = true;
        }
        else if(tankScript.HealthFactor > 0.3f && isplaying == true)
        {
            stopsmoke();
        }
        else if (tankScript.HealthFactor <= 0 && isplaying == true)
        {
            stopsmoke();
        }
    }
    private void stopsmoke()
    {
        engineSmoke.Stop();
        isplaying = false;
    }
}
