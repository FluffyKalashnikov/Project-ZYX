using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gamemode : ScriptableObject
{
    public string DisplayName  =  "GAMEMODE";
    public string Name         =  "GMODE";
    public float  Time         =  320f;
    public float  Score        =  10f;
    [Space(10)]
    public GameObject[] Prefabs;

//  TANK LOGIC
    public virtual void OnTankKill (Tank Tank, DamageInfo DamageInfo)
    {
        Debug.Log($"[{Name}]: Tank Died!");
    }
    public virtual void OnTankSpawn(Tank Tank)
    {
        Debug.Log($"[{Name}]: Tank Spawned!");
    }

//  MATCH LOGIC
    public virtual void BeginPlay()
    {
        Debug.Log($"[{Name}]: Begun playing!");
    }
    public virtual void Tick()
    {

    }
    public virtual void StopPlay()
    {
        Debug.Log($"[{Name}]: Stopped playing!");
    }
}
