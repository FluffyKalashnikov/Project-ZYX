﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gamemode : ScriptableObject
{
    [Header("Gamemode")]
    [SerializeField] protected string DisplayName  =  "GAMEMODE";
    [SerializeField] protected string Name         =  "GMODE";
    [SerializeField] protected float  MatchLength  =  320f;
    [SerializeField] protected float  Score        =  10f;
    [SerializeField] protected GameObject[] Prefabs;
    
    private IEnumerator IE_Exec = null;
    private IEnumerator IE_Main = null;

//  ** NOTES **
//  Destruct has to be called at 
//  the end of the lifecycle.
//

//  TANK LOGIC
    protected virtual void OnTankKill (Tank Tank, DamageInfo DamageInfo)
    {
        Debug.Log($"[{Name}]: Tank Died!");
    }
    protected virtual void OnTankSpawn(Tank Tank)
    {
        Debug.Log($"[{Name}]: Tank Spawned!");
    }

//  EVENTS
    protected virtual IEnumerator BeginPlay()
    {
        Debug.Log($"[{Name}]: Begun playing!");
        yield return null;
    }
    protected virtual IEnumerator Tick()
    {
        yield return null;
    }
    protected virtual IEnumerator StopPlay()
    {
        Debug.Log($"[{Name}]: Stopped playing!");
        yield return null;
    }
    
//  LIFE CYCLE
    public  void        Init()
    {
        Game.OnTankKill  += OnTankKill;
        Game.OnTankSpawn += OnTankSpawn;

        Game.Instance.StartCoroutine(IE_Exec = Exec());
        Debug.Log("[FFA]: Initialized.");
    }
    private void        Destruct()
    {
        Game.OnTankKill  -= OnTankKill;
        Game.OnTankSpawn -= OnTankSpawn;

        if (IE_Exec != null) Game.Instance.StopCoroutine(IE_Exec);
        if (IE_Main != null) Game.Instance.StopCoroutine(IE_Main);
        Debug.Log("[FFA]: Destroyed.");
    }
    private IEnumerator Exec()
    {
        float time = Time.time;

        Game.Instance.StartCoroutine(BeginPlay());
        yield return new WaitWhile
        (
            () => 
            { 
                Tick(); 
                return Time.time < time + MatchLength; 
            }
        );
        StopPlay();
        Destruct();
    }
}
