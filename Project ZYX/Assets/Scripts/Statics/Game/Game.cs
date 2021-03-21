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
    public HorizontalLayoutGroup PreviewRootUI = null;



    public static int ReadyCount = 0;
    private ActionAsset actionAsset = null;


    public static event Action       OnStartMatch;
    public static event Action       OnEndMatch   = () => MatchCleanup();
    public static event Action       OnStartLobby = () => InputManager.EnableJoining();
    public static event Action       OnEndLobby   = () => InputManager.DisableJoining();
    public static event Action       OnPause;
    public static event Action       OnUnpause;
    
    public static List<Tank>         PlayerList     = new List<Tank>();
    public static List<Tank>         AliveList      = new List<Tank>();
    public        List<Color>        PlayerColors   = new List<Color>(4);
    public static EGameState         GameState      = EGameState.Menu;
    public static EInputMode         InputMode      = EInputMode.MenuUI;
    public static bool               Paused         = false;
    public static Camera             Camera         = null;
    public static PlayerInputManager InputManager   = null;
    public static Game               Instance       = null;

    private static Widget MainMenuWidget = null;
    private static Widget PauseWidget    = null;
    private static Widget MatchWidget    = null;
    private static Widget LobbyWidget    = null;

    [SerializeField] private Button buttonMainMenuStart     = null;
    [SerializeField] private Button buttonPauseMenuContinue = null;
    [SerializeField] private Button buttonPauseMenuLobby    = null;

    public static CinemachineTargetGroup   CameraTargets = null;
    public static CinemachineVirtualCamera MatchCamera   = null;
    public static CinemachineVirtualCamera LobbyCamera   = null;

    public enum EGameState
    {
        Match,
        Lobby,
        Menu
    }
    public enum EInputMode
    {
        LobbyUI,
        MatchUI,
        MenuUI
    }
    public enum EGameFocus
    {
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
            if (GameState != EGameState.Match)
            return;
            switch(Paused = !Paused)
            {
                case true:  PauseGame();  return;
                default:    PauseReset(); return;
            }
        };

        // 4. EVENT SUBSCRIPTION
        buttonMainMenuStart    .onClick.AddListener(() => SwitchGameState(EGameState.Lobby));
        buttonPauseMenuContinue.onClick.AddListener(() => PauseReset());
        buttonPauseMenuLobby   .onClick.AddListener(() => SwitchGameState(EGameState.Lobby));

        OnStartMatch += ResetReady;
    }
    private void Start()
    {   
        SwitchInputMode(Tank.EInputMode.Menu);
        Widget.SetSelectedWidget(MainMenuWidget);
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

//  GAMEMODE LOGIC
    public static void SpawnTank(Tank tank, float delay = 0f)
    {
        Instance.StartCoroutine(Spawn());
        IEnumerator Spawn()
        {
            tank.DisableTank();
            yield return new WaitForSeconds(delay);
            tank.Controller.transform.position = RespawnPosition();
            tank.EnableTank();

            Vector3 RespawnPosition()
            {
                GameObject[] points = GameObject.FindGameObjectsWithTag("Respawn Point");
                switch (AliveList.Count)
                {
                    case 0: return points[UnityEngine.Random.Range(0, points.Length)].transform.position;
                    default: return FarthestPoint().transform.position;
                }


                GameObject FarthestPoint()
                {
                    // 1. Get list of players to compare
                    List<Tank> opponents = AliveList;
                    opponents.Remove(tank);

                    // VARIABLES
                    float maxDist = 0f;
                    int maxIndx = 0;

                    // 2. Save index for farthest point
                    for (int indx = 0; indx < points.Length; indx++)
                    {
                        float dist = Distance(points[indx]);

                        if (maxDist < dist)
                        {
                            maxDist = dist;
                            maxIndx = indx;
                        }
                    }

                    // 3. Return farthest point
                    return points[maxIndx];

                    float Distance(GameObject point)
                    {
                        float closest = Mathf.Infinity;

                        // Stores closest distance to a player
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
            }
        }
    }
    public static void OnTankDie(Tank tank, MonoBehaviour dealer)
    {

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
    public static void SelectWidget(Widget widget, bool ignoreNull = false)
    {
        Widget.SetSelectedWidget(widget, ignoreNull);
    }
    public static void PauseGame()
    {
        Paused = true;
        Widget.AddWidget(PauseWidget);
        PauseWidget.SetSelectedElement(PauseWidget.defaultElement);
        OnPause?.Invoke();
    }
    public static void PauseReset()
    {
        Paused = false;
        Widget.RemoveOverlays();
        OnUnpause?.Invoke();
    }
    public static void ResetReady()
    {
        ReadyCount = 0;
        foreach (var i in PlayerList)
        i.Ready = false;
    }

//  INPUT LOGIC
    public static void SwitchInputMode(Tank.EInputMode InputMode)
    {
        Tank.SwitchInputMode(InputMode);
    }

//  MATCH LOGIC
    public static void StartMatch()
    {
        // 1. SET INPUT/STATE
        OnStartMatch?.Invoke();
        SwitchGameState(EGameState.Match);
        SwitchInputMode(Tank.EInputMode.Game);

        // 2. MATCH LOGIC
        SelectWidget(MatchWidget);
        Cam.SetActiveCamera(MatchCamera);
        SpawnTanks();
        PauseReset();

        Debug.Log("Match Started!");
    }
    public static void StartLobby()
    {
        // 1. SET INPUT/STATE
        OnStartLobby?.Invoke();
        SwitchGameState(EGameState.Lobby);
        SwitchInputMode(Tank.EInputMode.Lobby);

        // 2. LOBBY LOGIC
        SelectWidget(LobbyWidget);
        Cam.SetActiveCamera(LobbyCamera);
        PauseReset();
        DisableTanks();

        Debug.Log("Lobby Started!");
    }
    public static void MatchCleanup()
    {
        DisableTanks();

        Debug.Log($"Cleanup Finished!");
    }
    public static void SwitchGameFocus(EGameFocus GameFocus)
    {
        switch(GameFocus)
        {
            case EGameFocus.Lobby: break;
            case EGameFocus.Match: break;
            case EGameFocus.Menu:  break;
            case EGameFocus.Pause: break;
        }
    }

//  APPLICATION LOGIC
    public static void Exit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
