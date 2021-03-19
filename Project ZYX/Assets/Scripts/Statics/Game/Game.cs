using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class Game : MonoBehaviour
{
    [Header("Game properties")]
    private static Widget MainMenuWidget = null;
    private static Widget PauseWidget    = null;
    private static Widget MatchWidget    = null;
    private static Widget LobbyWidget    = null;

                     private ActionAsset actionAsset = null;
    [Header("Static Setters")]
    [SerializeField] private List<Color> playerColors = new List<Color>(4);

    public static event Action       OnStartMatch;
    public static event Action<Tank> OnWinMatch;
    public static event Action       OnStartLobby;

    public static event Action<PlayerInput> OnPlayerJoin = player => PlayerJoin(player, player.GetComponent<Tank>());
    public static event Action<PlayerInput> OnPlayerLeft = player => PlayerLeft(player, player.GetComponent<Tank>());

    
    public static List<Tank>         PlayerList     = new List<Tank>();
    public static List<Tank>         AliveList      = new List<Tank>();
    public static List<Color>        PlayerColors   = new List<Color>();
    public static GameState          State          = GameState.Empty;
    public static bool               Paused         = false;
    public static Camera             Camera         = null;
    public static PlayerInputManager InputManager   = null;
    public static Game               Instance       = null;

    public static CinemachineTargetGroup   CameraTargets = null;
    public static CinemachineVirtualCamera MatchCamera   = null;
    public static CinemachineVirtualCamera LobbyCamera   = null;

    public enum GameState
    {
        Empty,
        Match,
        Lobby,
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

        // 2. STATIC SETTERS
        PlayerColors = playerColors;

        // 3. REFERENCES
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


        // 4. INPUT SETUP
        actionAsset = new ActionAsset();
        actionAsset.IngameUI.Enable();

        // 5. EVENT SUBSCRIPTION
        InputManager.onPlayerJoined += OnPlayerJoin;
        InputManager.onPlayerLeft   += OnPlayerLeft;
    }
    private void Start()
    {
        Widget.SetSelected(MainMenuWidget);
    }
    



//  PLAYER LOGIC
    public static void PlayerJoin(PlayerInput player, Tank tank)
    {
        
    }
    public static void PlayerLeft(PlayerInput player, Tank tank)
    {
        
    }

//  GAMEMODE LOGIC
    public static void SpawnTank(Tank tank, float delay = 0f)
    {
        Instance.StartCoroutine(Spawn());
        IEnumerator Spawn()
        {
            tank.Disable();
            yield return new WaitForSeconds(delay);
            tank.Controller.transform.position = RespawnPosition();
            tank.Enable();

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
        {
            SpawnTank(tank, delay);
        }
    }
    public static void EnableTanks()
    {
        foreach (Tank tank in FindObjectsOfType<Tank>())
        {
            tank.Enable();
        }
    }
    public static void DisableTanks()
    {
        foreach (Tank tank in FindObjectsOfType<Tank>())
        {
            tank.Disable();
        }
    }
    public static void SetGameState(GameState State)
    {
        // 1. CHECK NEW STATE
        if (Game.State == State)
        return;

        // 2. END OLD LOGIC
        switch(Game.State)
        {
            case GameState.Match: OnEndMatch(); break;
            case GameState.Lobby: OnEndLobby(); break;
            case GameState.Menu:  break;
            default: Debug.LogError($"GameState \"{State}\" is not supported!"); return;
        }

        // 3. BEGIN NEW LOGIC
        switch(Game.State = State)
        {
            case GameState.Match: StartMatch(); break;
            case GameState.Lobby: StartLobby(); break;
            case GameState.Menu:  break;
            default: Debug.LogError($"GameState \"{State}\" is not supported!"); return;
        }
    }

//  MENU LOGIC
    public static void SelectWidget(Widget widget, bool ignoreNull = false)
    {
        Widget.SetSelected(widget, ignoreNull);
    }
    public static void PauseGame()
    {
        if (Game.State != GameState.Match) return;
        if (Paused) return;

        SelectWidget(PauseWidget);

        Time.timeScale = 0;
    }
    public static void ResumeGame()
    {
        if (Game.State != GameState.Match) return;
        if (!Paused) return;

        SelectWidget(MatchWidget);

        Time.timeScale = 1;
    }

//  MATCH LOGIC
    public static void StartMatch()
    {
        OnStartMatch?.Invoke();
        SetGameState(GameState.Match);

        SelectWidget(MatchWidget);
        SetActiveCamera(MatchCamera);
        SpawnTanks();

        Debug.Log("Match Started!");
    }
    public static void OnEndMatch()
    {
        MatchCleanup();
    }
    public static void StartLobby()
    {
        OnStartLobby?.Invoke();
        SetGameState(GameState.Match);

        SelectWidget(LobbyWidget);
        SetActiveCamera(LobbyCamera);

        Debug.Log("Lobby Started!");
    }
    public static void OnEndLobby()
    {

    }
    public static void MatchCleanup()
    {
        DisableTanks();

        Debug.Log($"Cleanup Finished!");
    }
    public static void WinMatch(Tank winner)
    {
        OnWinMatch(winner);
    }
    
//  CAMERA LOGIC
    public static void SetActiveCamera(CinemachineVirtualCamera Camera)
    {
        Cam.SetActiveCamera(Camera);
    }

//  APPLICATION LOGIC
    public static void Exit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
