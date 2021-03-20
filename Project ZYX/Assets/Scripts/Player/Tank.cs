using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(MultiplayerEventSystem), typeof(InputSystemUIInputModule), typeof(PlayerInput))]
public class Tank : MonoBehaviour, IDamageable
{
    [Header("Variables")]
    public string Name          = "Player 1";
    public int    MaxNameLength = 18;
    public float  MaxHealth     = 100f;
    public float  Health        = 0;
    
    public bool   Alive         = true;
    public bool   Ready         = false;
    public Color  Color 
    {
        get {return Game.Instance.PlayerColors[PlayerInput.playerIndex];}
    }
    
    [Header("REFERENCES")]
    public TankMovement           TankMovement       = null;
    public TankShoot              TankShoot          = null;
    public TankTurret             TankTurret         = null;
    [Space(10)]
    public CharacterController    Controller         = null;
    public PlayerInput            PlayerInput        = null; 
    public MultiplayerEventSystem LocalEventSystem   = null;
    [Space(10)]
    public MeshRenderer[]         TankRenderers      = null;
    [Space(10)]
    public GameObject             PreviewPrefab      = null;
    public GameObject             PreviewInstance    = null;
    [Space(5)]
    public Button                 PreviewButtonReady = null;
    public Button                 PreviewButtonRight = null;
    public Button                 PreviewButtonLeft  = null;
    public TMP_InputField         PreviewNameField   = null;
    public RawImage               PreviewTankImage   = null;
    public Image                  PreviewBackground  = null;
    
    IEnumerator IE_ResetSelect = null;

    [SerializeField]
    private Image[] imagesToColor = null;

    public static       Action<Tank> OnTankFire;
    public static event Action<Tank> OnTankDeath = tank => tank.Die();

    [Header("Unity Events")]
    public UnityEvent OnEnable;
    public UnityEvent OnDisable;


    public int Power = 0;
    public float PowerUpTimer;

    public enum EInputMode
    {
        Game,
        Lobby,
        Menu
    }



    public void Awake()
    {
        // 1. REFERENCES
        this.TankMovement     = GetComponent<TankMovement>();
        this.TankShoot        = GetComponent<TankShoot>();
        this.TankTurret       = GetComponent<TankTurret>();
        this.LocalEventSystem = GetComponent<MultiplayerEventSystem>();
        this.TankRenderers    = GetComponentsInChildren<MeshRenderer>(true);

        // 2. INIT LOGIC
        InitPlayer();
        DisableTank();
        UpdateTank();
        InitPreview();

        // 3. EVENT SUBSCRIPTION
        Game.OnStartLobby += EnablePreview;
        Game.OnEndLobby   += DisablePreview;
        Game.OnStartMatch += () => Ready = false;
        
        Game.OnStartMatch += () => SwitchInputMode(EInputMode.Game);
        Game.OnStartLobby += () => SwitchInputMode(EInputMode.Lobby);
        Game.OnPause      += () => SwitchInputMode(EInputMode.Menu);
    }

//  TANK SETUP
    public void UpdateTank()
    {
        UpdateReferences();
        UpdateColor();
    }
    public void UpdateColor()
    {
        foreach (var i in TankRenderers)
        {
            i.material.color = Color;
        }
    }
    public void UpdateReferences()
    {

    }
    
//  TANK HEALTH
    public float TakeDamage(float damage, DamageInfo info, MonoBehaviour dealer)
    {
        PowerUpTimer -= Time.deltaTime;
        if (PowerUpTimer < 0)
        {
            Power = 0;
        }
        if (Power <= 0)
        {
            damage = 0;
        }
        if ((Health -= damage) <= 0f)
        {
            Die();
            Game.OnTankDie(this, dealer);
        }

        return damage;
    }
    public void Die()
    {
        StartCoroutine(DeathEffect());
        IEnumerator DeathEffect()
        {
            yield return new WaitForSeconds(3f);
            Debug.Log($"{this.name} has died!");
            DisableTank();
        }
    }

//  TANK STATE
    public void EnableTank()
    {
        if (Alive) return;

        Game.AliveListAddPlayer(this);
        Game.CameraTargets.AddMember(Controller.transform, 1f, 0f);
        ShowTank();

        OnEnable.Invoke();
        Alive = true;
    }
    public void DisableTank()
    {
        if (!Alive) return;

        Game.AliveListRemovePlayer(this);
        Game.CameraTargets.RemoveMember(Controller.transform);
        HideTank();

        OnDisable.Invoke();
        Alive = false;
    }
    public void HideTank()
    {
        foreach (var i in TankRenderers)
        i.forceRenderingOff = true;
    }
    public void ShowTank()
    {
        foreach (var i in TankRenderers)
        i.forceRenderingOff = false;
    }

//  USER INTERFACE
    public void InitPreview()
    {
        // 1. CREATE INSTANCE
        {
            var root = Instantiate
            (
                PreviewPrefab, 
                Game.Instance.PreviewRootUI.transform
            );
            LocalEventSystem.playerRoot = root;

            // 2. GET REFERENCES
            PreviewNameField  = root.GetComponentInChildren<TMP_InputField>(true);
            PreviewTankImage  = root.GetComponentInChildren<RawImage>(true);
            PreviewBackground = root.GetComponent<Image>();
            PreviewInstance   = root;
            {
                var i = root.GetComponentsInChildren<Button>(true);
                PreviewButtonReady = i[0];
                PreviewButtonRight = i[1];
                PreviewButtonLeft  = i[2];
            }
        }

        // 3. UPDATE COLOR
        PreviewBackground.color = Color;

        // 4. SUBSCRIBE EVENTS
        PreviewNameField.onValidateInput += NameValidation;
        PreviewNameField.onSubmit.AddListener(NameSubmit);
        PreviewNameField.onSelect.AddListener(NameSelect);

        PreviewButtonReady.onClick.AddListener(() => Game.UpdateReady(this));
        
        // 5. FINAL SETUP
        DisablePreview();
        if (Game.GameState == Game.EGameState.Lobby)
        EnablePreview();
    }
    public void EnablePreview()
    {
        PreviewInstance.SetActive(true);
        LocalEventSystem.SetSelectedGameObject(null);
        LocalEventSystem.sendNavigationEvents = true;
        ResetPreviewSelection();
    }
    public void DisablePreview()
    {
        PreviewInstance.SetActive(false);
        LocalEventSystem.SetSelectedGameObject(null);
        LocalEventSystem.sendNavigationEvents = false;
    }
    public void ResetPreviewSelection()
    {
        if (IE_ResetSelect != null)
        StopCoroutine (IE_ResetSelect);
        StartCoroutine(IE_ResetSelect = Logic());

        IEnumerator Logic()
        {
            yield return new WaitUntil(() => PreviewButtonReady != null && PreviewButtonReady.enabled);
            LocalEventSystem.SetSelectedGameObject(PreviewButtonReady.gameObject);
            PreviewNameField.DeactivateInputField(false);
        }
    }

    private char NameValidation(string text, int charIndex, char addedChar)
    {
        // 1. RETURN NULL IF TOO LONG
        if (text.Length >= MaxNameLength)
            return '\0';
        // 2. RETURN CHAR IF VALID
        if (char.IsLetterOrDigit(addedChar) || addedChar == ' ' || addedChar == '_')
            return addedChar;
        // 3. ELSE RETURN TERMINATION CHAR
        return '\0';
    }
    private void NameSubmit(string text)
    {
        // 1. UPDATE PLAYER NAME
        Name = text.Trim();

        // 2. UNSELECT INPUT FIELD
        PreviewNameField.SetTextWithoutNotify(Name);
        PreviewNameField.DeactivateInputField();
        PreviewNameField.caretWidth = 0;
        ResetPreviewSelection();
    }
    private void NameSelect(string text)
    {
        PreviewNameField.caretWidth = 1;
    }

//  PLAYER STATE
    public void InitPlayer()
    {
        gameObject.name = name = $"Player {PlayerInput.playerIndex+1}";
        Health = MaxHealth;
        Game.PlayerList.Add(this);
    }
    public void SwitchInputMode(EInputMode InputMode)
    {
        switch(InputMode)
        {
            case EInputMode.Game: 
                PlayerInput.SwitchCurrentActionMap("Player");
                // GLOBAL
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.sendNavigationEvents = false;
                // LOCAL
                LocalEventSystem.SetSelectedGameObject(null);
                LocalEventSystem.sendNavigationEvents = false;

            break;
            
            case EInputMode.Lobby:
                PlayerInput.SwitchCurrentActionMap("UI");
                // GLOBAL
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.sendNavigationEvents = false;
                // LOCAL
                ResetPreviewSelection();
                LocalEventSystem.sendNavigationEvents = true;
            break;

            case EInputMode.Menu:
                PlayerInput.SwitchCurrentActionMap("UI");
                // GLOBAL
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.sendNavigationEvents = true;
                // LOCAL
                LocalEventSystem.SetSelectedGameObject(null);
                LocalEventSystem.sendNavigationEvents = false;

            break;
        }
    }
}
