using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerInputManager))]
public class Game : MonoBehaviour
{
    // GAME PROPERTIES
    public static int          ReadyCount = 0;
    public static EState       State
    {
        get { return m_State; }
        set 
        { 
            // 1. BAIL IF SET
            if (m_State == value)
            return;
            EState OldState = m_State;
            EState NewState = m_State = value;
            
            // 2. BEGIN END LOGIC
            switch(OldState)
            {
                case EState.Pause: RemoveWidget(PauseWidget); break;
                default: break;
            }
            // 3. BEGIN NEW LOGIC
            switch(NewState)
            {
                case EState.MainMenu:  SetActiveWidget(MenuWidget);  break;
                case EState.Match:     SetActiveWidget(MatchWidget); break;
                case EState.Lobby:     SetActiveWidget(LobbyWidget); break;
                case EState.Pause:     AddActiveWidget(PauseWidget); break;
                case EState.WinScreen: SetActiveWidget(WinWidget);   break;
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


    private static EState      m_State     = EState.Empty;

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
    private static Widget WinWidget   = null;
    
    // REFERENCES
    public static Camera                   Camera                  = null;
    public static PlayerInputManager       InputManager            = null;
    public static Game                     Instance                = null;
    public        HorizontalLayoutGroup    PreviewRootUI           = null;
    public static CinemachineTargetGroup   CameraTargets           = null;
    public static CinemachineVirtualCamera MatchCamera             = null;
    public static CinemachineVirtualCamera LobbyCamera             = null;
    private static WidgetSwitcher          MenuSwitch              = null;
    private static WidgetSwitcher          PopupSwitch             = null;
    private static IEnumerator             IE_Count                = null;
    public         TextMeshProUGUI         CountdownText           = null;
    public         List<Scoreboard>        Scoreboards             = new List<Scoreboard>();
    public         TextMeshProUGUI         WinText                 = null;
    

    public enum EState
    {
        Empty,
        Match,
        Lobby,
        Pause,
        MainMenu,
        WinScreen
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
        {
            var i = GetComponentsInChildren<WidgetSwitcher>(true);
            MenuSwitch  = i[0];
            PopupSwitch = i[1];
        }
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
            WinWidget   = i[4];
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
            EnableJoining();
            MatchCleanup();
        };
        WinWidget  .OnEnabled = () =>
        {
            SetActiveInput(Tank.EInputMode.Menu);
            UpdateWinText();
        };

        MenuWidget .OnDisabled = () =>
        {

        };
        PauseWidget.OnDisabled = () =>
        {
            Time.timeScale = 1f;
        };
        MatchWidget.OnDisabled = () =>
        { 
            CurrentMode.Destruct();
        };
        LobbyWidget.OnDisabled = () =>
        {
            DisableJoining();
        };
        WinWidget  .OnDisabled = () =>
        {
            Game.MatchCleanup();
        };

        // 4. MISC
        CurrentMode = ModeList[1];
    }
    private void Start() => LoadMainMenu();
    
    
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
    public static void ResetScore()
    {
        foreach (var i in PlayerList)
        i.Score = 0;
        UpdateScoreboards();
    }
    public static void EnableJoining()
    {
        Game.InputManager.EnableJoining();
    }
    public static void DisableJoining()
    {
        Game.InputManager.DisableJoining();
    }

//  RESPAWN LOGIC
    public static void SpawnTank(Tank Tank)
    {
        if (!Tank.Alive)
        RespawnTank(Tank);
    }
    public static void SpawnTanks()
    {
        foreach (Tank Tank in PlayerList)
        SpawnTank(Tank);
    }
    public static void RespawnTank(Tank Tank)
    {
        // 1. DISABLE/ENABLE TANK
        Tank.DisableTank();
        
        // 2. SET POSITION/ROTATION
        Spawnpoint Spawnpoint = FindSpawnpoint();
        Tank.Position = Spawnpoint.Position;
        Tank.Rotation = Spawnpoint.Rotation;
        
        Debug.Log($"SPAWN POSITION: {Tank.Position}");
        OnTankSpawn?.Invoke(Tank);
    }
    public static void RespawnTanks()
    {
        foreach (Tank tank in PlayerList)
        RespawnTank(tank);
    }

    public static Spawnpoint[] FindSpawnpoints()
    {
        return Spawnpoint.List.ToArray();
    }
    public static Spawnpoint   FindSpawnpoint()
    {
        switch(AliveList.Count)
        {
            case 0:  return FindRandomSpawnpoint();
            default: return FindFurthestSpawnpoint();
        }
    }
    public static Spawnpoint   FindFurthestSpawnpoint()
    {
        // VARIABLES
        Spawnpoint[] Spawnpoints = FindSpawnpoints();
        Spawnpoint   Storedpoint = Spawnpoints[0];

        foreach (var Otherpoint in Spawnpoints)
        {
            Storedpoint = Otherpoint.Distance > Storedpoint.Distance 
            ? Otherpoint
            : Storedpoint;
        }

        Debug.Log($"[GAME]: Found furthest point. ({Storedpoint.name})");
        return Storedpoint;
    }
    public static Spawnpoint   FindRandomSpawnpoint()
    {
        Spawnpoint[] Spawnpoints = FindSpawnpoints();
        int          max         = Spawnpoints.Length;
        Spawnpoint   Storedpoint = null;
        
        // 1. GET
        Storedpoint = max != 0
        ? Spawnpoints[UnityEngine.Random.Range(0, max)]
        : null;

        // 2. RETURN
        Debug.Log($"[GAME]: Random point found. ({Storedpoint.name})");
        return Storedpoint;
    }

//  GAMEMODE LOGIC
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
    public static void EnableInput()
    {
        foreach (var i in PlayerList)
        i.EnableInput();
    }
    public static void DisableInput()
    {
        foreach (var i in PlayerList)
        i.DisableInput();
    }
    public static void EnableLookOnly()
    {
        foreach (var i in PlayerList)
        i.EnableLookOnly();
    }

    public static void BeginMatch()
    {
        SetActiveState(EState.Match);
        UpdateScoreboards();
        MatchCleanup();
        CurrentMode.Init();
        OnNewMatch?.Invoke();
    }
    public static void StopMatch()
    {

    }
    public static void BeginLobby()
    {
        SetActiveState(EState.Lobby);
        OnNewLobby?.Invoke();
    }
    public static void StopLobby()
    {
        OnEndLobby?.Invoke();
    }
    
//  MENU LOGIC
    public static void PauseGame()
    {
        SetActiveState(EState.Pause);
        OnPause?.Invoke();
    }
    public static void ResumeGame()
    {
        SetActiveState(EState.Match);
        SetActiveInput(Tank.EInputMode.Game);
        OnResume?.Invoke();
    }
    public static void ResetOverlay()
    {
        Widget.RemoveOverlays();
        OnPauseReset?.Invoke();
        MatchCleanup();
    }
    public static void ResetReady()
    {
        ReadyCount = 0;
        foreach (var i in PlayerList)
        i.Ready = false;
    }
    public static void LoadMainMenu()
    {
        SetActiveState(EState.MainMenu);
    }

//  SETTERS
    public static void SetActiveCamera(CinemachineVirtualCamera Camera)
    {
        Cam.SetActiveCamera(Camera);
    }
    public static void SetActiveInput(Tank.EInputMode InputMode)
    {
        Tank.SwitchInputMode(InputMode);
    }
    public static void SetActiveState(EState State)
    {
        Game.State = State;
    }

//  WIDGET LOGIC
    public static void AddActiveWidget(Widget Widget)
    {
        Widget.AddWidget(Widget);
    }
    public static void SetActiveWidget(Widget Widget)
    {
        MenuSwitch.SetActiveWidget(Widget);
    }
    public static void RemoveWidget(Widget Widget)
    {
        Widget.RemoveWidget(Widget);
    }
    public static void AddCountdown(float Time)
    {
        if (!IsPlaying()) return;
        if (IE_Count != null)
        MatchContext.I.StopCoroutine(IE_Count);
        MatchContext.I.StartCoroutine(IE_Count = Logic());

        IEnumerator Logic()
        {
            Game.Instance.CountdownText.gameObject.SetActive(true);
            while (Time > 0)
            {
                int   min = Mathf.FloorToInt(Time/60f);
                float sec = Time - (float) min * 60f;

                Game.Instance.CountdownText.SetText
                (
                    min == 0 
                     ? $"{Time}"
                     : $"{min}:{(sec >= 10 ? $"{sec}" : $"0{sec}")}"
                );
                Time--;
                yield return new WaitForSeconds(1f);
            }
            Game.Instance.CountdownText.gameObject.SetActive(false);
        }
    }

//  MATCH LOGIC/
    public static void MatchCleanup()
    {
        // 1. DEACTIVATE PLAYERS
        MatchContext.Stop();
        DisableTanks();
        
        // 2. DELETE OBJECTS
        foreach (var i in GameObject.FindGameObjectsWithTag("Disposable"))
        Destroy(i);

        Debug.Log($"[GAME]: Cleanup Finished!");
    }
    public static void UpdateScoreboards()
    {
        foreach(var i in Instance.Scoreboards)
        {
            i.Validate();
            i.UpdateWidgets();
            i.Sort();
        }
    }
    public static void SortScoreboards()
    {
        foreach(var i in Instance.Scoreboards)
        i.Sort();
    }
    public static void ResetScoreboard()
    {
        foreach (var i in Instance.Scoreboards)
        i.Reset();
    }
    public static void UpdateWinText()
    {
        // 1. GET WINNERS
        List<Tank> Winners   = GetWinners();
        string     WinString = string.Empty; 

        // 2. BUILD STRING
        switch (Winners.Count)
        {
            // 3. IF SINGLE WINNER, SET
            case 1: WinString = $"{Winners[0].Name} won!"; break;
            default:
            // 4. IF MULTIPLE, BUILD STRING
                WinString = $"{Winners[0].Name}";
                for (int i = 1; i < Winners.Count-1; i++)
                {
                    WinString += $", {Winners[i].Name}";
                }
                WinString += $" and {Winners[Winners.Count-1].Name} won!";
            break;
        }

        // 5. SET TEXT
        Instance.WinText.SetText(WinString);



        List<Tank> GetWinners()
        {
            // 1. GET HIGHEST SCORE
            float highestScore = 0;
            foreach (var player in PlayerList)
            {
                highestScore = player.Score > highestScore
                 ? player.Score
                 : highestScore;
            }

            // 2. RETURN PLAYERS WITH SCORE
            return PlayerList.FindAll(tank => tank.Score >= highestScore);
        }
    }

//  APPLICATION LOGIC
    public static void Exit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
    public static bool IsPaused()
    {
        return State == EState.Pause;
    }
    public static bool IsPlaying()
    {
        return State == EState.Match;
    }
}
