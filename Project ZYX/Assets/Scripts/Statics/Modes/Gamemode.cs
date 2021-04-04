using System.Collections;
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
    private bool initialized = false;

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
    protected virtual void BeginPlay()
    {
        Debug.Log($"[{Name}]: Begun playing!");
    }
    protected virtual void Tick()
    {

    }
    protected virtual void StopPlay()
    {
        Debug.Log($"[{Name}]: Stopped playing!");
    }
    
//  LIFE CYCLE
    public void Init()
    {
        if (!initialized) return;
        initialized = true;

        Game.OnTankKill  += OnTankKill;
        Game.OnTankSpawn += OnTankSpawn;

        Game.Instance.StartCoroutine(IE_Exec = Exec());
        Debug.Log("[FFA]: Initialized.");
    }
    public void Destruct()
    {
        if (initialized) return;
        initialized = false;

        Game.OnTankKill  -= OnTankKill;
        Game.OnTankSpawn -= OnTankSpawn;

        if (IE_Exec != null) Game.Instance.StopCoroutine(IE_Exec);
        if (IE_Main != null) Game.Instance.StopCoroutine(IE_Main);
    }
    protected virtual IEnumerator Main()
    {
        yield return null;
    }
    private IEnumerator Exec()
    {
        float time = Time.time;

        BeginPlay();
        Game.Instance.StartCoroutine(IE_Main = Main());
        yield return new WaitWhile
        (
            () => 
            { 
                Tick(); 
                return Time.time < time + MatchLength; 
            }
        );
        StopPlay();
    }
}
