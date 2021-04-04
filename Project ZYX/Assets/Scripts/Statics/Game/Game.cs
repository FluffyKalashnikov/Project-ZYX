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
    public VerticalLayoutGroup ScoreboardLayout = null;
    public HorizontalLayoutGroup PreviewRootUI = null;
    public static int ReadyCount = 0;
    private ActionAsset actionAsset = null;

    public static List<Tank>  PlayerList   = new List<Tank>();
    public static List<Tank>  AliveList    = new List<Tank>();
    public static TankAsset[] TankTypes
    {
        get { return Resources.LoadAll<TankAsset>("Tank Assets"); }
    }
    public static Gamemode[]  ModeList
    {
        get { return Resources.LoadAll<Gamemode>("Mode Assets"); }
    }
    public        Gamemode    Gamemode;
    public        List<Color> PlayerColors = new List<Color>(4);
    public static List<ScoreboardElement> ScoreElements = new List<ScoreboardElement>();

    public static EGameState  GameState = EGameState.Empty;
    public static EInputMode  InputMode = EInputMode.Empty;
    public static EGameFocus  GameFocus = EGameFocus.Empty;


    // GAME EVENTS
    public static event Action OnNewMatch;
    public static event Action OnEndMatch;
    public static event Action OnNewLobby;
    public static event Action OnEndLobby;

    public static event Action OnPause;
    public static event Action OnPauseReset;
    public static event Action OnResume;

    public static event Action TickMatch;

    // GAME WIDGETS
    private static Widget MainMenuWidget = null;
    private static Widget PauseWidget    = null;
    private static Widget MatchWidget    = null;
    private static Widget LobbyWidget    = null;
    
    // REFERENCES
    public static Camera             Camera       = null;
    public static PlayerInputManager InputManager = null;
    public static Game               Instance     = null;

    [SerializeField] private Button buttonMainMenuStart     = null;
    [SerializeField] private Button buttonPauseMenuContinue = null;
    [SerializeField] private Button buttonPauseMenuLobby    = null;

    public static CinemachineTargetGroup   CameraTargets = null;
    public static CinemachineVirtualCamera MatchCamera   = null;
    public static CinemachineVirtualCamera LobbyCamera   = null;

    public enum EGameState
    {
        Empty,
        Match,
        Lobby,
        Menu
    }
    public enum EInputMode
    {
        Empty,
        LobbyUI,
        MatchUI,
        MenuUI
    }
    public enum EGameFocus
    {
        Empty,
        Match,
        Lobby,
        Menu,
        Pause
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
        Camera        = GetComponentInChildren<Camera>();
        CameraTargets = GetComponentInChildren<CinemachineTargetGroup>(); 
        {
            var i = GetComponentsInChildren<CinemachineVirtualCamera>();
            MatchCamera = i[0];
            LobbyCamera = i[1];
        }
        {
            var i = GetComponentsInChildren<Widget>();
            MatchWidget    = i[0];
            LobbyWidget    = i[1];
            MainMenuWidget = i[2];
            PauseWidget    = i[3];
        }

        // 3. INPUT SETUP
        actionAsset = new ActionAsset();
        actionAsset.IngameUI.Enable();

        actionAsset.IngameUI.Pause.performed += ctx => 
        { 
            if (GameFocus == EGameFocus.Match || GameFocus == EGameFocus.Pause)
            switch(GameFocus)
            {
                case EGameFocus.Match: PauseGame();  break;
                case EGameFocus.Pause: ResumeGame(); break;
                default: 
                    Debug.LogError($"[GAME]: EGameFocus \"{GameFocus}\"not implemented."); 
                return;
            }
        };

        // 4. EVENT SUBSCRIPTION
        OnNewMatch += () => TickMatch += Gamemode.Tick;
        OnEndMatch += () => TickMatch -= Gamemode.Tick;
        OnNewMatch += Gamemode.BeginPlay;
        OnEndMatch += Gamemode.StopPlay;

        OnNewLobby += InputManager.EnableJoining;
        OnEndLobby += InputManager.DisableJoining;

        buttonMainMenuStart    .onClick.AddListener(() => SwitchGameState(EGameState.Lobby));
        buttonPauseMenuContinue.onClick.AddListener(() => ResumeGame());
        buttonPauseMenuLobby   .onClick.AddListener(() => SwitchGameState(EGameState.Lobby));
    }
    private void Start()
    {   
        SwitchGameFocus
        (
            EGameFocus.Menu, () => 
            {
                Widget.SetSelectedWidget(MainMenuWidget, null);
            }
        );
    }
    private void Update()
    {
        TickMatch?.Invoke();
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
            StartMatch();
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
    public static void OnTankKill(Tank Tank, DamageInfo DamageInfo)
    {
        Instance.Gamemode?.OnTankKill(Tank, DamageInfo);
    }
    public static void OnTankSpawn(Tank Tank)
    {
        Instance.Gamemode?.OnTankSpawn(Tank);
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
        foreach (Tank tank in FindObjectsOfType<Tank>())
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
    public static void SwitchGameState(EGameState GameState)
    {
        // 1. CHECK NEW STATE
        if (Game.GameState == GameState)
        return;
        // 2. END OLD LOGIC
        switch(Game.GameState)
        {
            case EGameState.Match:  OnEndMatch(); break;
            case EGameState.Lobby:  OnEndLobby(); break;
            case EGameState.Menu:   break;
        }
        // 3. BEGIN NEW LOGIC
        switch(Game.GameState = GameState)
        {
            case EGameState.Match:  StartMatch(); break;
            case EGameState.Lobby:  StartLobby(); break;
            case EGameState.Menu:   break;
        }
    }

//  MENU LOGIC
    public static void SelectWidget(Widget widget, Action OnComplete = null, bool ignoreNull = false)
    {
        Widget.SetSelectedWidget(widget, OnComplete, ignoreNull);
    }
    public static void PauseGame()
    {
        Widget.AddWidget
        (
            PauseWidget, () => 
            {
                SwitchGameFocus
                (
                    EGameFocus.Pause, () => 
                    {
                        PauseWidget.SetSelectedElement
                        (
                            PauseWidget.defaultElement, () => 
                            {
                                Time.timeScale = 0f;
                                OnPause?.Invoke();
                            }
                        );
                    }
                );
            }
        );
        
    }
    public static void PauseReset(Action OnComplete)
    {
        Widget.RemoveOverlays
        (
            () => 
            {
                Time.timeScale = 1f;
                OnPauseReset?.Invoke();
                OnComplete?.Invoke();
            }
        );
    }
    public static void ResumeGame()
    {
        PauseReset
        (
            () => 
            {
                SwitchGameFocus
                (
                    EGameFocus.Match, () => 
                    {
                        OnResume?.Invoke();
                    }
                );
            }
        );
    }
    public static void ResetReady()
    {
        ReadyCount = 0;
        foreach (var i in PlayerList)
        i.Ready = false;
    }

//  INPUT LOGIC
    private static void SwitchInputMode(Tank.EInputMode InputMode, Action OnComplete)
    {
        Tank.SwitchInputMode(InputMode, OnComplete);
    }

//  MATCH LOGIC
    public static void StartMatch()
    {
        SelectWidget
        (
            MatchWidget, () =>
            {
                SwitchGameFocus
                (
                    EGameFocus.Match, () => 
                    {
                        PauseReset(null);
                        InitScoreboard();
                        Cam.SetActiveCamera(MatchCamera);

                        SwitchGameState(EGameState.Match);
                        OnNewMatch?.Invoke();
                    }
                );
            }
        );
    }
    public static void StartLobby()
    {
        SelectWidget
        (
            LobbyWidget, () => 
            {
                SwitchGameFocus
                (
                    EGameFocus.Lobby, () => 
                    {
                        DisableTanks();
                        PauseReset(null);
                        Cam.SetActiveCamera(LobbyCamera);

                        SwitchGameState(EGameState.Lobby);
                        OnNewLobby?.Invoke();
                    }
                );
            }
        );
    }
    public static void MatchCleanup()
    {
        DisableTanks();

        Debug.Log($"[GAME]: Cleanup Finished!");
    }
    public static void SwitchGameFocus(EGameFocus GameFocus, Action OnComplete)
    {
        if (Game.GameFocus != GameFocus)
        switch(Game.GameFocus = GameFocus)
        {
            case EGameFocus.Lobby: SwitchInputMode(Tank.EInputMode.Lobby, OnComplete); break;
            case EGameFocus.Match: SwitchInputMode(Tank.EInputMode.Game,  OnComplete); break;
            case EGameFocus.Menu:  SwitchInputMode(Tank.EInputMode.Menu,  OnComplete); break;
            case EGameFocus.Pause: SwitchInputMode(Tank.EInputMode.Menu,  OnComplete); break;
        }
    }
    public static void InitScoreboard()
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
        ScoreboardElement.SortElements();
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
        return GameFocus == EGameFocus.Pause;
    }
    public static bool IsPlaying()
    {
        return GameFocus == EGameFocus.Match;
    }
}
