using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance = null;

    public UnityEvent OnPause;
    public UnityEvent OnUnpause;

    private ActionAsset actionAsset = null;
    private Widget pauseWidget = null;

    

    private void Awake() 
    {
        if (!Instance) Instance = this;
        actionAsset = new ActionAsset();
        actionAsset.Player.Enable();

        actionAsset.Player.Pause.performed += ctx => 
        {
            if (Game.State == Game.EState.Match)
            {
                OnPause.Invoke();
            }
            else if (Game.State == Game.EState.Paused)
            {
                OnUnpause.Invoke();
            }
        };

        OnPause  .AddListener(Pause);
        OnUnpause.AddListener(Unpause);

        pauseWidget = Widget.Find("Pause UI");
    }



    
    public static void SetActiveWidget(Widget widget)
    {
        Widget.SetSelected(widget);
    }
    

    private void Pause()
    {
        Debug.Log("Pausing...");
        Game.State = Game.EState.Paused;
        Widget.AddWidget(pauseWidget);
    }
    private void Unpause()
    {
        Debug.Log("Unpausing...");
        Game.State = Game.EState.Match;
        Widget.RemoveWidget(pauseWidget);
    }
}

