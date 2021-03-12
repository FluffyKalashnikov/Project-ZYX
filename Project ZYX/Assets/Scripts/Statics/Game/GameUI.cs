using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance = null;

    public UnityEvent OnPause;
    public UnityEvent OnUnpause;

    private ActionAsset actionAsset = null;
    [SerializeField] private Canvas[] everyCanvas = null;
    


    private void Awake() 
    {
        if (!Instance) Instance = this;
        actionAsset = new ActionAsset();
        actionAsset.Player.Enable();

        actionAsset.Player.Pause.performed += ctx => 
        {
            Debug.Log("Bröd");
            if (Game.State == Game.GameState.Match)
            {
                OnPause.Invoke();
            }
            else if (Game.State == Game.GameState.Paused)
            {
                OnUnpause.Invoke();
            }
        };

        OnPause  .AddListener(Pause);
        OnUnpause.AddListener(Unpause);
    }
    



    public static void SetActiveUI(GameObject UI)
    {
        foreach (var i in FindObjectsOfType<Canvas>())
        {
            i.gameObject.SetActive(false);
        }

        UI.SetActive(true);
    }

    private void Pause()
    {
        Debug.Log("Pausing...");
        Game.State = Game.GameState.Paused;
    }
    private void Unpause()
    {
        Debug.Log("Unpausing...");
        Game.State = Game.GameState.Match;
    }
}
