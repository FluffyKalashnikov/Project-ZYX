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
    //[Header("Game properties")]
    [Header("Static Setters")]
    [SerializeField] private List<Color> playerColors = new List<Color>(4);

    [Header("Unity Events")]
    [SerializeField] private UnityEvent onStartMatch;
    [SerializeField] private UnityEvent onWinMatch;
    [SerializeField] private UnityEvent onStartLobby;

    public static event Action       OnStartMatch = () => Instance.onStartMatch.Invoke();
    public static event Action<Tank> OnWinMatch   = wn => Instance.onWinMatch  .Invoke();
    public static event Action       OnStartLobby = () => Instance.onStartLobby.Invoke();

    public static event Action<PlayerInput> OnPlayerJoin = player => PlayerJoin(player, player.GetComponent<Tank>());
    public static event Action<PlayerInput> OnPlayerLeft = player => PlayerLeft(player, player.GetComponent<Tank>());

    
    public static List<Tank>             PlayerList    = new List<Tank>();
    public static List<Tank>             AliveList     = new List<Tank>();
    public static List<Color>            PlayerColors  = new List<Color>();
    public static GameState              State         = GameState.Empty;

    public static Camera                 Camera        = null;
    public static PlayerInputManager     InputManager  = null;
    public static Game                   Instance      = null;
    public static CinemachineTargetGroup CameraTargets = null;

    public enum GameState
    {
        Empty,
        Match,
        Lobby,
        Menu,
        Paused
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
        CameraTargets = GetComponentInChildren<CinemachineTargetGroup>();
        Camera        = GetComponentInChildren<Camera>();

        // 4. EVENT SUBSCRIPTION

        InputManager.onPlayerJoined += OnPlayerJoin;
        InputManager.onPlayerLeft   += OnPlayerLeft;
    }
    
    
    
    public static void PlayerJoin(PlayerInput player, Tank tank)
    {
        PlayerList.Add(tank);
    }
    public static void PlayerLeft(PlayerInput player, Tank tank)
    {
        PlayerList.Add(tank);
    }

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
    

    public static void StartMatch()
    {
        OnStartMatch?.Invoke();
        State = GameState.Match;

        SpawnTanks();

        Debug.Log("Match Started!");
    }
    public static void StartLobby()
    {
        OnStartLobby?.Invoke();
        State = GameState.Lobby;
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
    
    public static void Exit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
