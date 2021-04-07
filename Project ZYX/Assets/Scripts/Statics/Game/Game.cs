using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputManager))]
public class Game : MonoBehaviour
{
    // GAME PROPERTIES
    public static int          ReadyCount = 0;
    public static EFocus       Focus
    {
        get { return m_State; }
        set 
        { 
            // 1. BAIL IF SET
            if (m_State == value)
            return;
            // 2. BEGIN NEW LOGIC
            switch(m_State = value)
            {
                case EFocus.Menu:  SetActiveWidget(MenuWidget);  break;
                case EFocus.Match: SetActiveWidget(MatchWidget); break;
                case EFocus.Lobby: SetActiveWidget(LobbyWidget); break;   
                //case EFocus.Pause: SetActiveWidget(PauseWidget); break;
                case EFocus.Pause: AddActiveWidget(PauseWidget); break;
            }
        }
    }
    
    public static List<Tank>   PlayerList   = new List<Tank>();
    public static List<Tank>   AliveList    = new List<Tank>();
    public static TankAsset[]  TankTypes
    {
        get { return Resources.LoadAll<TankAsset>("Tank Assets"); }
    }
    public static Gamemode[]   ModeList
    {
        get { return Resources.LoadAll<Gamemode>("Mode Assets"); }
    }
    public static Gamemode     CurrentMode;
    public        List<Color>  PlayerColors = new List<Color>(4);
    public static List<ScoreWidget> ScoreElements = new List<ScoreWidget>();


    private static EFocus      m_State     = EFocus.Empty;

    // GAME EVENTS
    public static event Action OnNewMatch;
    public static event Action OnEndMatch;
    public static event Action OnNewLobby;
    public static event Action OnEndLobby;

    public static event Action OnPause;
    public static event Action OnPauseReset;
    public static event Action OnResume;

    public static Action<Tank, DamageInfo> OnTankKill; 
    public static Action<Tank>             OnTankSpawn;

    // GAME WIDGETS
    private static Widget MenuWidget  = null;
    private static Widget PauseWidget = null;
    private static Widget MatchWidget = null;
    private static Widget LobbyWidget = null;
    
    // REFERENCES
    public static Camera                   Camera                  = null;
    public static PlayerInputManager       InputManager            = null;
    public static Game                     Instance                = null;
    [SerializeField] private Button        buttonMainMenuStart     = null;
    [SerializeField] private Button        buttonPauseMenuContinue = null;
    [SerializeField] private Button        buttonPauseMenuNewLobby = null;
    public        VerticalLayoutGroup      ScoreboardLayout        = null;
    public        HorizontalLayoutGroup    PreviewRootUI           = null;
    public static CinemachineTargetGroup   CameraTargets           = null;
    public static CinemachineVirtualCamera MatchCamera             = null;
    public static CinemachineVirtualCamera LobbyCamera             = null;
    private static WidgetSwitcher          MenuSwitch              = null;

    public enum EFocus
    {
        Empty,
        Match,
        Lobby,
        Pause,
        Menu
    }

    private void Awake()
    {
        // 1. SINGLETON
        if (Instance == null) Instance = this;
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 2. REFERENCES
        InputManager  = GetComponent<PlayerInputManager>();
        Camera        = GetComponentInChildren<Camera>(true);
        CameraTargets = GetComponentInChildren<CinemachineTargetGroup>(true); 
        MenuSwitch    = GetComponentInChildren<WidgetSwitcher>(true);
        {
            var i = GetComponentsInChildren<CinemachineVirtualCamera>();
            MatchCamera = i[0];
            LobbyCamera = i[1];
        }
        {
            var i = GetComponentsInChildren<Widget>(true);
            MatchWidget = i[0];
            LobbyWidget = i[1];
            MenuWidget  = i[2];
            PauseWidget = i[3];
        }

        // 3. INIT WIDGETS
        MenuWidget .OnEnabled = () =>
        {
            SetActiveInput(Tank.EInputMode.Menu);
            SetActiveCamera(LobbyCamera);
        };
        PauseWidget.OnEnabled = () =>
        {
            SetActiveInput(Tank.EInputMode.Menu);
            Time.timeScale = 0f;
            OnPause?.Invoke();

            Debug.Log("Pause Enabled.");
        };
        MatchWidget.OnEnabled = () =>
        {
            SetActiveInput(Tank.EInputMode.Game);
            SetActiveCamera(MatchCamera);
        };
        LobbyWidget.OnEnabled = () =>
        {
            SetActiveInput(Tank.EInputMode.Lobby);
            SetActiveCamera(LobbyCamera);
        };

        MenuWidget .OnDisabled = () =>
        {
            
        };
        PauseWidget.OnDisabled = () =>
        {
            PauseReset();
            Debug.Log("Pause Disabled.");
        };
        MatchWidget.OnDisabled = () =>
        {
            
        };
        LobbyWidget.OnDisabled = () =>
        {
            
        };


        // 4. EVENT SUBSCRIPTION
        OnNewLobby += InputManager.EnableJoining;
        OnEndLobby += InputManager.DisableJoining;

        // 5. MISC
        CurrentMode = ModeList[1];
    }
    private void Start()
    {   
        SetActiveFocus(EFocus.Menu);
    }
    
//  PLAYER LOGIC
    public static void UpdateReady(Tank tank)
    {
        // 1. UPDATE COUNT
        switch(tank.Ready = !tank.Ready)
        {
            case true:  ReadyCount++; break;
            case false: ReadyCount--; break;
        }

        // 2. CHECK COUNT
        if (ReadyCount >= PlayerList.Count)
        {
            BeginMatch();
            ResetReady();
        }
    }
    public static void AliveListAddPlayer(Tank tank)
    {
        if (!AliveList.Contains(tank))
        AliveList.Add(tank);
    }
    public static void AliveListRemovePlayer(Tank tank)
    {
        if (AliveList.Contains(tank))
        AliveList.Remove(tank);
    }
    public static void ResetScore()
    {
        foreach (var i in PlayerList)
        i.Score = 0;
        UpdateScoreboard();
    }

//  GAMEMODE LOGIC
    public static void SpawnTank(Tank tank, float delay = 0f)
    {
        Instance.StartCoroutine(Spawn());
        IEnumerator Spawn()
        {
            tank.DisableTank();
            yield return new WaitForSeconds(delay);
            tank.Controller.transform.position = RespawnPosition();
            tank.OnSpawned();

            Vector3 RespawnPosition()
            {
                GameObject[] points = GameObject.FindGameObjectsWithTag("Respawn Point");
                switch (AliveList.Count)
                {
                    case 0:  return RandomPoint  ().transform.position;
                    default: return FarthestPoint().transform.position;
                }

                GameObject FarthestPoint()
                {
                    // 1. GET OPPONENTS
                    List<Tank> opponents = AliveList;
                    opponents.Remove(tank);

                    // 2. SAVE FURTHEST INDEX
                    float maxDistance = 0f;
                    int   maxIndex    = 0;

                    for (int index = 0; index < points.Length; index++)
                    {
                        float distance = GetDistance(points[index]);

                        if (maxDistance < distance)
                        {
                            maxDistance = distance;
                            maxIndex    = index;
                        }
                    }

                    // 3. RETURN FURTHEST POINT
                    return points[maxIndex];

                    float GetDistance(GameObject point)
                    {
                        float closest = Mathf.Infinity;

                        // STORE CLOSEST DISTANCE
                        foreach (var opponent in opponents)
                        {
                            float current = Vector3.Distance
                            (
                                point.transform.position,
                                opponent.Controller.transform.position
                            );
                            closest = current < closest ? current : closest;
                        }
                        return closest;
                    }
                }
                GameObject RandomPoint()
                {
                    return points[UnityEngine.Random.Range(0, points.Length)];
                }
            }
        }
    }
    public static void SpawnTanks(float delay = 0f)
    {
        foreach (Tank tank in PlayerList)
        SpawnTank(tank, delay);
    }
    public static void EnableTanks()
    {
        foreach (Tank tank in FindObjectsOfType<Tank>())
        tank.EnableTank();
    }
    public static void DisableTanks()
    {
        foreach (Tank tank in FindObjectsOfType<Tank>())
        tank.DisableTank();
    }
   
    public static void BeginMatch()
    {
        SetActiveFocus(EFocus.Match);
        UpdateScoreboard();
        CurrentMode.Init();
        OnNewMatch?.Invoke();
    }
    public static void StopMatch()
    {

    }
    public static void BeginLobby()
    {
        SetActiveFocus(EFocus.Lobby);
        OnNewLobby?.Invoke();
    }
    public static void StopLobby()
    {
        OnEndLobby?.Invoke();
    }
    
//  MENU LOGIC
    public static void PauseGame()
    {
        SetActiveFocus(EFocus.Pause);
    }
    public static void ResumeGame()
    {
        SetActiveFocus(EFocus.Match);
        OnResume?.Invoke();
    }
    public static void PauseReset()
    {
        Widget.RemoveOverlays();
        Time.timeScale = 1f;
        OnPauseReset?.Invoke();
    }
    public static void ResetReady()
    {
        ReadyCount = 0;
        foreach (var i in PlayerList)
        i.Ready = false;
    }

//  SETTERS
    public static void AddActiveWidget(Widget Widget)
    {
        Widget.AddWidget(Widget);
    }
    public static void SetActiveWidget(Widget Widget)
    {
        Widget.RemoveOverlays();
        MenuSwitch.SetActiveWidget(Widget);
    }
    public static void SetActiveCamera(CinemachineVirtualCamera Camera)
    {
        Cam.SetActiveCamera(Camera);
    }
    public static void SetActiveInput(Tank.EInputMode InputMode)
    {
        Tank.SwitchInputMode(InputMode);
    }
    public static void SetActiveFocus(EFocus Focus)
    {
        Game.Focus = Focus;
    }

//  MATCH LOGIC
    public static void MatchCleanup()
    {
        DisableTanks();

        Debug.Log($"[GAME]: Cleanup Finished!");
    }
    public static void UpdateScoreboard()
    {
        // 1. CREATE SCOREBOARDS
        foreach(var i in PlayerList)
        {
            // 1. IF EXIST
            if (i.ScoreElement)
            i.ScoreElement.UpdateElement();
            // 2. ELSE CREATE
            i.InitScore();
        } 
        // 2. DESTROY UNPARENTED SCOREBOARDS
        foreach(var i in ScoreElements)
        {
            if (i.Owner && i.Owner.ScoreElement == i) 
            continue;
            Destroy(i.gameObject);
        }
        // 3. SORT
        SortScoreboard();
    }
    public static void SortScoreboard()
    {
        ScoreWidget.SortElements();
    }
    public static void RemoveScoreboard()
    {
        for (int i = ScoreElements.Count; i >= 0; i--)
        Destroy(ScoreElements[i].gameObject);
    }

//  APPLICATION LOGIC
    public static void Exit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
    public static bool IsPaused()
    {
        return Focus == EFocus.Pause;
    }
    public static bool IsPlaying()
    {
        return Focus == EFocus.Match;
    }
}
