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
    public static EState       State
    {
        get { return m_State; }
        set 
        { 
            // 1. BAIL IF SET
            if (m_State == value)
            return;
            // 2. END OLD LOGIC
            switch(m_State)
            {
                case EState.Match:  Game.OnEndMatch?.Invoke(); break;
                case EState.Lobby:  Game.OnEndLobby?.Invoke(); break;
                case EState.Menu:   break;
            }
            // 3. BEGIN NEW LOGIC
            switch(m_State = value)
            {
                case EState.Match:  Game.StartMatch(); break;
                case EState.Lobby:  Game.StartLobby(); break;
                case EState.Menu:   break;
            }
        }
    }
    public static EFocus       Focus
    {
        get { return m_Focus; }
        set 
        {   
            // 1. BAIL IF SET
            if (m_Focus == value)
            return;
            // 2. SWITCH INPUT MODE
            switch(m_Focus = value)
            {
                case EFocus.Lobby: SwitchInputMode(Tank.EInputMode.Lobby); break;
                case EFocus.Match: SwitchInputMode(Tank.EInputMode.Game);  break;
                case EFocus.Menu:  SwitchInputMode(Tank.EInputMode.Menu);  break;
                case EFocus.Pause: SwitchInputMode(Tank.EInputMode.Menu);  break;
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
    private static EFocus      m_Focus     = EFocus.Empty;

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
    private static Widget MainMenuWidget = null;
    private static Widget PauseWidget    = null;
    private static Widget MatchWidget    = null;
    private static Widget LobbyWidget    = null;
    
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
    private       ActionAsset              actionAsset             = null;

    public enum EState
    {
        Empty,
        Match,
        Lobby,
        Menu
    }
    public enum EFocus
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
            if (Focus == EFocus.Match || Focus == EFocus.Pause)
            switch(Focus)
            {
                case EFocus.Match: PauseGame();  break;
                case EFocus.Pause: ResumeGame(); break;
                default: 
                    Debug.LogError($"[GAME]: EGameFocus \"{Focus}\"not implemented."); 
                return;
            }
        };

        // 4. EVENT SUBSCRIPTION
        OnNewLobby += InputManager.EnableJoining;
        OnEndLobby += InputManager.DisableJoining;

        buttonMainMenuStart    .onClick.AddListener(() => Game.State = EState.Lobby);
        buttonPauseMenuContinue.onClick.AddListener(() => ResumeGame());
        buttonPauseMenuNewLobby.onClick.AddListener(() => Game.State = EState.Lobby);

        // 5. MISC
        CurrentMode = ModeList[1];
    }
    private void Start()
    {   
        Game.Focus = EFocus.Menu;
        Widget.SetSelectedWidget(MainMenuWidget, null);
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

//  MENU LOGIC
    public static void SelectWidget(Widget widget, Action OnComplete = null, bool ignoreNull = false)
    {
        Widget.SetSelectedWidget(widget, OnComplete, ignoreNull);
    }
    public static void PauseGame()
    {
        Widget.AddWidget(PauseWidget);
        Game.Focus = EFocus.Pause;
        PauseWidget.SetSelectedElement
        (
            PauseWidget.defaultElement, () => 
            {
                Time.timeScale = 0f;
                OnPause?.Invoke();
            }
        );
    }
    public static void PauseReset()
    {
        Widget.RemoveOverlays();
        Time.timeScale = 1f;
        OnPauseReset?.Invoke();
    }
    public static void ResumeGame()
    {
        PauseReset();
        Game.Focus = EFocus.Match;
        OnResume?.Invoke();
    }
    public static void ResetReady()
    {
        ReadyCount = 0;
        foreach (var i in PlayerList)
        i.Ready = false;
    }

//  INPUT LOGIC
    private static void SwitchInputMode(Tank.EInputMode InputMode)
    {
        Tank.SwitchInputMode(InputMode);
    }

//  MATCH LOGIC
    public static void StartMatch()
    {
        SelectWidget
        (
            MatchWidget, () =>
            {
                Game.Focus = EFocus.Match; 
                PauseReset();
                UpdateScoreboard();
                Cam.SetActiveCamera(MatchCamera);
                CurrentMode.Init();

                Game.State = EState.Match;
                OnNewMatch?.Invoke();
            }
        );
    }
    public static void StartLobby()
    {
        SelectWidget
        (
            LobbyWidget, () => 
            {
                Focus = EFocus.Lobby;
                DisableTanks();
                PauseReset();
                Cam.SetActiveCamera(LobbyCamera);

                Game.State = EState.Lobby;
                OnNewLobby?.Invoke();
            }
        );
    }
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
