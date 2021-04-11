using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gamemode : ScriptableObject
{
    [Header("Gamemode")]
    [SerializeField] protected string DisplayName  =  "GAMEMODE";
    [SerializeField] protected string Name         =  "GMODE";
    [SerializeField] protected float  MatchLength  =  320f;
    [SerializeField] protected GameObject[] Prefabs;

//  ** NOTES **
//  Destruct has to be called at 
//  the end of the lifecycle.
//

//  TANK LOGIC
    protected virtual IEnumerator OnTankKill (Tank Tank, DamageInfo DamageInfo)
    {
        Debug.Log($"[{Name}]: Tank Died!");
        yield return null;
    }
    protected virtual IEnumerator OnTankSpawn(Tank Tank)
    {
        Debug.Log($"[{Name}]: Tank Spawned!");
        yield return null;
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
    
//  EVENT STARTERS
    private void StartTankKill(Tank Tank, DamageInfo DamageInfo)
    {
        MatchContext.Add(OnTankKill(Tank, DamageInfo));
    }
    private void StartTankSpawn(Tank Tank)
    {
        MatchContext.Add(OnTankSpawn(Tank));
    }

//  LIFE CYCLE
    protected void StopGame()
    {
        MatchContext.Add(StopPlay());
    }
    public    void Init()
    {
        Game.OnTankKill  += StartTankKill;
        Game.OnTankSpawn += StartTankSpawn;

        MatchContext.Add(Exec());
        Debug.Log($"[{Name}]: Initialized.");
    }
    public    void Destruct()
    {
        Game.OnTankKill  -= StartTankKill;
        Game.OnTankSpawn -= StartTankSpawn;

        Game.OnEndMatch?.Invoke();
        MatchContext.Stop();

        Debug.Log($"[{Name}]: Destroyed.");
    }

    private   IEnumerator Exec()
    {
        float time = Time.time;
        MatchContext.Add(BeginPlay());
        Game.OnNewMatch?.Invoke();
        yield return new WaitWhile
        (
            () => 
            { 
                // LOOPS UNTIL COROUTINE STOPPED
                MatchContext.Add(Tick()); 
                return true; 
            }
        );
    }
}
