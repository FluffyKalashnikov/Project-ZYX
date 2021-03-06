using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInputManager))]
public class OLD_Game : MonoBehaviour
{
    public Color[] playerColors;
    public static OLD_Game           Instance      =  null;
    public static PlayerInputManager inputManager  =  null;

    public static event Action<PlayerInput> OnPlayerJoin;
    public static event Action<PlayerInput> OnPlayerLeft;

    [SerializeField] private UnityEvent OnStartMatch;
    [SerializeField] private UnityEvent OnEndMatch;
    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
        Instance = this;


        inputManager.onPlayerJoined += PI => StartCoroutine(JoinBehaviour(PI));
        inputManager.onPlayerLeft   += PI => StartCoroutine(LeftBehaviour(PI)); 

        IEnumerator JoinBehaviour(PlayerInput playerInput)
        {
            yield return new WaitUntil(() => playerInput == true);


            // Adds created player to list
            OLD_PLR.PlayerList.Add(playerInput.GetComponent<OLD_PLR>());


            OnPlayerJoin?.Invoke(playerInput);
        }
        IEnumerator LeftBehaviour(PlayerInput playerInput)
        {
            // Removes deleted player from list
            OLD_PLR.PlayerList.Remove(playerInput.GetComponent<OLD_PLR>());

            yield return new WaitUntil(() => playerInput == false);

            OnPlayerLeft?.Invoke(playerInput);
        }
    }
    private void Start()
    {
        OnPlayerJoin += i => PLR_UI.UpdateUI();
        OnPlayerLeft += i => PLR_UI.UpdateUI();
    }



    public static void StartMatch()
    {
        Debug.Log("MATCH STARTED.");

        PLR_UI.CloseUI();
        foreach (var i in OLD_PLR.PlayerList) i.LoadTank();
        Instance.OnStartMatch.Invoke();
    }
    public static void EndMatch()
    {
        Debug.Log("MATCH ENDED.");
        PLR_UI.OpenUI();
        Instance.OnEndMatch.Invoke();
    }
}
