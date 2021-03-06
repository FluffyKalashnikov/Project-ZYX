using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PLR_UI : MonoBehaviour
{
    [SerializeField] private Canvas         canvas          = null;
    [SerializeField] private Image[]        coloredElements = new Image[0];
    [SerializeField] private RectTransform  menuTransform   = null;
    [SerializeField] private Button         readyButton     = null;
    [SerializeField] private Button         rightButton     = null;
    [SerializeField] private Button         leftButton      = null;

    public Action OnReady;
    public Action OnUnready;

    // VARIABLES
    [HideInInspector] public bool ready = false;



    private void Awake()
    {
        readyButton.onClick.AddListener(() => { Ready(); });
        OLD_Game.OnPlayerJoin += UpdateColor;
    }



    private void UpdateColor(PlayerInput player)
    {
        var script = player.GetComponent<PLR_UI>();

        foreach (var i in script.coloredElements)
        {
            i.color = OLD_Game.Instance.playerColors[player.playerIndex];
        }
    }
    private void Ready()
    {
        switch (ready = !ready)
        {
            case true:
                OnReady?.Invoke();

                // Gets ready players
                int amount = 0;
                foreach (var i in OLD_PLR.PlayerList)
                {
                    if (i.UI.ready) amount++;
                }

                // If everyone ready, start
                if (amount == OLD_PLR.PlayerList.Count)
                {
                    OLD_Game.StartMatch();
                }
                break;
            case false: OnUnready?.Invoke(); break;
        }
    }




    public static void UpdateUI()
    {
        foreach (var player in OLD_PLR.PlayerList)
        {
            float width = player.UI.canvas.pixelRect.width / player.UI.canvas.scaleFactor;
            float index = player.playerInput.playerIndex + 1;
            float offset = width / (OLD_PLR.PlayerList.Count + 1);


            player.UI.menuTransform.anchoredPosition3D = new Vector3
            (
                offset * index - width / 2,
                player.UI.menuTransform.anchoredPosition3D.y,
                0f
            );
        }
    }
    public static void OpenUI()
        {
            foreach (var player in OLD_PLR.PlayerList)
            {
                player.UI.canvas.enabled = true;
            }
        }
    public static void CloseUI()
        {
            foreach (var player in OLD_PLR.PlayerList)
            {
                player.UI.canvas.enabled = false;
            }
        }
}
