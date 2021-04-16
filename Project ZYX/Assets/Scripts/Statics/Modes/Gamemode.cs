using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gamemode : ScriptableObject
{
    [Header("Gamemode")]
    [SerializeField] protected string DisplayName  =  "GAMEMODE";
    [SerializeField] protected string Name         =  "GMODE";
    [SerializeField] protected float  MatchLength  =  320f;

    [SerializeField] 
    protected      GameObject WidgetPrefab = null;
    protected      GameObject Widget
    {
        get { return m_Widget; }
        set
        {
            // 1. BAIL IF SET
            if (value == null || m_Widget == value) 
            return;
            // 2. REPLACE WIDGET
            if (m_Widget) Destroy(m_Widget);
            m_Widget = Instantiate(value, Game.MatchWidget.transform);
            // 3. PLACE WIDGET
            var i = m_Widget.GetComponent<RectTransform>();
            //i.anchorMax = new Vector2()
        }
    }
    private static GameObject m_Widget     = null;
    protected static bool     Playing      = false;
    
//  INIT LOGIC
    protected virtual void InitWidget(GameObject Widget)
    {
        
    }

//  TANK LOGIC
    protected virtual IEnumerator OnTankKill (Tank Tank, DamageInfo DamageInfo)
    {
        Debug.Log($"[{Name}]: Tank Died!");

        yield return new WaitForSeconds(3f);
        Game.SpawnTank(Tank);
    }
    protected virtual IEnumerator OnTankSpawn(Tank Tank)
    {
        Debug.Log($"[{Name}]: Tank \"{Tank.Name}\" Spawned!");
        Tank.Health = Tank.MaxHealth;
        Tank.Enable();
        Tank.OpenHUD();
        yield return null;
    }
    
//  EVENTS
    protected virtual IEnumerator BeginPlay()
    {
        Debug.Log($"[{Name}]: Begun playing!");
        Game.RespawnTanks();
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
        Debug.Log($"[{Name}]: Match Initialized.");
    }
    public    void Destruct()
    {
        Game.OnTankKill  -= StartTankKill;
        Game.OnTankSpawn -= StartTankSpawn;

        
        MatchContext.Stop();
        Playing = false;

        Debug.Log($"[{Name}]: Match Destroyed.");
    }
    public static bool IsPlaying()
    {
        return Playing;
    }

    private   IEnumerator Exec()
    {
        float time = Time.time;
        Playing = false;
        Widget = WidgetPrefab;
        InitWidget(Widget);
        MatchContext.Add(BeginPlay());
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
