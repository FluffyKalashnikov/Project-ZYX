﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.Animations;
using TMPro;

[RequireComponent(typeof(MultiplayerEventSystem), typeof(InputSystemUIInputModule), typeof(PlayerInput))]
public class Tank : MonoBehaviour, IDamageable
{
    public TankAsset  TankAsset
    {
        get 
        { 
            return m_TankAsset 
             ? m_TankAsset 
             : m_TankAsset = Game.Instance.TankTypes[TankIndex]; 
        }
        set 
        { 
            if (m_TankAsset != value)
            LoadStats(m_TankAsset = value); 
        }
    }
    public GameObject Model
    {
        get { return m_Model; }
        set 
        {
            // REPLACE MODEL
            if (m_Model) Destroy(m_Model);
            m_Model = Instantiate(value, Controller ? Controller.transform : transform);
            m_Model.transform.position = Vector3.zero;
            UpdateTank();
        }
    }
    public TankRef    TankRef
    {
        get 
        {
            return m_TankRef ? m_TankRef : m_TankRef = Model.GetComponent<TankRef>();
        }
    }

    public float      MaxHealth
    {
        get { return TankAsset ? TankAsset.Health : 42069f; }
    }
    public float      Damage
    {
        get { return TankAsset ? TankAsset.Damage : 42069; }
    }
    public float      Speed
    {
        get { return TankAsset ? TankAsset.Speed : 42069; }
    }

    public AudioClip  AudioIdle
    {
        get { return TankAsset ? TankAsset.AudioIdle : null; }
    }
    public AudioClip  AudioStartup
    {
        get { return TankAsset ? TankAsset.AudioStartup : null; }
    }
    public AudioClip  AudioThrottle
    {
        get { return TankAsset ? TankAsset.AudioThrottle : null; }
    }
    
    [Header("Player Bools")]
    public bool       Alive = true;
    public bool       Ready = false;
    public Color      Color 
    {
        get {return Game.Instance.PlayerColors[PlayerIndex];}
    }
    [Header("Naming properties")]
    public int        MaxNameLength = 18;
    public List<char> SpecialChars  = new List<char>();


    // PROPERTIES
    public float  Health
    {
        get {return m_Health;}
        set {m_Health = Mathf.Clamp(value, 0, MaxHealth); UpdateHealthbar(); }
    }
    public float  HealthFactor
    {
        get {return Health/MaxHealth;}
    }
    public string Name 
    {
        get 
        {
            if (m_Name == string.Empty)
            Name = $"Player {PlayerIndex+1}";
            return m_Name;
        }
        set {m_Name = value; ScoreElement?.UpdateElement();}
    }
    public float  Score
    {
        get { return m_Score; }
        set { m_Score = value; Game.SortScoreboard(); }
    }
    public int    PlayerIndex 
    {
        get {return PlayerInput.playerIndex;}
    }
    public int    TankIndex
    {
        get { return Mathf.Clamp(m_TankIndex, 0, Game.Instance.TankTypes.Count-1);  }
        set 
        { 
            // SET INDEX/UPDATE TANK
            m_TankIndex = Mathf.Clamp(value, 0, Game.Instance.TankTypes.Count-1); 
            TankAsset = Game.Instance.TankTypes.Count != 0 ? Game.Instance.TankTypes[m_TankIndex] : null;
        }
    }

    private GameObject m_Model     = null;
    private TankAsset  m_TankAsset = null;
    private TankRef    m_TankRef   = null;
    private float      m_Health    = 0f;
    private float      m_Score     = 0f;
    private string     m_Name      = string.Empty;
    private int        m_TankIndex = 0;

    [Header("REFERENCES")]
    #region references
    public TankMovement           TankMovement       = null;
    public TankShoot              TankShoot          = null;
    public TankTurret             TankTurret         = null;
    public TankAudio              TankAudio          = null;
    [Space(10)]
    public CharacterController    Controller         = null;
    public PlayerInput            PlayerInput        = null; 
    public MultiplayerEventSystem LocalEventSystem   = null;
    [Space(10)]
    public MeshRenderer[]         TankRenderers      = null;
    [Space(10)]
    public GameObject             PreviewPrefab      = null;
    public GameObject             PreviewInstance    = null;
    public GameObject             ScorePrefab        = null;
    public ScoreboardElement      ScoreElement       = null;
    [Space(5)]
    public Button                 PreviewButtonReady = null;
    public Button                 PreviewButtonRight = null;
    public Button                 PreviewButtonLeft  = null;
    public TMP_InputField         PreviewNameField   = null;
    public RawImage               PreviewTankImage   = null;
    public Image                  PreviewBackground  = null;
    #endregion

    public ParentConstraint HudConstraint = null;
    public GameObject       HudRoot       = null;
    public Image            HealthBar     = null;

    IEnumerator IE_ResetSelect = null;

    public Action OnTankFire;
    public Action OnTankDeath;


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
        this.TankAudio        = GetComponent<TankAudio>();
        this.LocalEventSystem = GetComponent<MultiplayerEventSystem>();

        // 2. INIT LOGIC
        InitPlayer();
        DisableTank();
        UpdateTank();
        InitPreview();
        InitHUD();

        // 3. EVENT SUBSCRIPTION
        Game.OnNewLobby += EnablePreview;
        Game.OnEndLobby += DisablePreview;
        Game.OnNewMatch += () => Ready = false;
    }

//  TANK SETUP
    public void UpdateTank()
    {
        UpdateReferences();
        UpdateColor();
    }
    public void UpdateReferences()
    {
        this.TankRenderers = GetComponentsInChildren<MeshRenderer>(true);
    }
    public void UpdateColor()
    {
        foreach (var rd in TankRenderers)
        {
            foreach(var i in rd.materials)
            i.color = Color;
        }
    }
    
//  TANK HEALTH
    public float TakeDamage(float damage, DamageInfo info, MonoBehaviour dealer)
    {
        /*
        PowerUpTimer -= Time.deltaTime;
        if (PowerUpTimer < 0)
        {
            Power = 0;
        }
        if (Power <= 0)
        {
            damage = 0;
        }
        */
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

//  TANK ENABLING
    public void EnableTank()
    {
        if (Alive) return;

        Game.AliveListAddPlayer(this);
        Game.CameraTargets.AddMember(Controller.transform, 1f, 0f);
        ShowTank();
        EnableHUD();

        Alive = true;
    }
    public void DisableTank()
    {
        if (!Alive) return;

        Game.AliveListRemovePlayer(this);
        Game.CameraTargets.RemoveMember(Controller.transform);
        HideTank();
        DisableHUD();

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

//  TANK STATS
    public void LoadStats(TankAsset Asset)
    {
        Model = Asset.Model;
        UpdateTank();

        TankMovement.OnLoadStats(TankRef);
        TankShoot   .OnLoadStats(TankRef);
        TankAudio   .OnLoadStats(TankRef);

        Debug.Log($"\"{Asset.Name}\" Tank has been loaded.");
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
        PreviewButtonRight.onClick.AddListener(() => TankIndex++);
        PreviewButtonLeft .onClick.AddListener(() => TankIndex--);
        
        // 5. FINAL SETUP
        DisablePreview();
        if (Game.GameState == Game.EGameState.Lobby)
        EnablePreview();
    }
    public void EnablePreview()
    {
        SwitchLocalInputMode(EInputMode.Menu);
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
            LocalEventSystem.SetSelectedGameObject(null);
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
        // 2. RETURN CHAR IF NORMAL
        if (char.IsLetterOrDigit(addedChar))
            return addedChar;
        // 3. RETURN IF SPECIAL CHAR
        if (SpecialChars.Contains(addedChar))
            return addedChar;
        // 4. ELSE RETURN TERMINATION CHAR
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
        EventSystem.current.SetSelectedGameObject(null);
        LocalEventSystem.SetSelectedGameObject(null);
        ResetPreviewSelection();
    }
    private void NameSelect(string text)
    {
        PreviewNameField.caretWidth = 1;
        PreviewNameField.caretPosition = PreviewNameField.text.Length;
    }

    public void InitScore()
    {
        // 1. CREATE SCOREBOARD
        ScoreboardElement i = Instantiate
        (
            ScorePrefab, 
            Game.Instance.ScoreboardLayout.transform
        ).GetComponent<ScoreboardElement>();

        // 2. INIT AND GET REF
        ScoreElement = i;
        i.Init(this);
    }

//  PLAYER STATE
    public void InitPlayer()
    {
        gameObject.name = $"Player {PlayerInput.playerIndex+1}";
        Health = MaxHealth;
        Game.PlayerList.Add(this);
        LoadStats(TankAsset);
    }
    public void SwitchLocalInputMode(EInputMode InputMode)
    {
        switch(InputMode)
        {
            case EInputMode.Game:  PlayerInput.SwitchCurrentActionMap("Player"); break;
            case EInputMode.Lobby: PlayerInput.SwitchCurrentActionMap("UI");     break;
            case EInputMode.Menu:  PlayerInput.SwitchCurrentActionMap("UI");     break;
        }
    }
    public static void SwitchInputMode(EInputMode InputMode, Action OnComplete)
    {
        switch(InputMode)
        {
            case EInputMode.Game: 
                foreach (var i in Game.PlayerList)
                i.SwitchLocalInputMode(InputMode);
                // GLOBAL
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.sendNavigationEvents = false;
                // LOCAL
                foreach (var i in Game.PlayerList)
                i.LocalEventSystem.SetSelectedGameObject(null);
                foreach (var i in Game.PlayerList)
                i.LocalEventSystem.sendNavigationEvents = false;

            break;
            
            case EInputMode.Lobby:
                foreach (var i in Game.PlayerList)
                i.SwitchLocalInputMode(InputMode);
                // GLOBAL
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.sendNavigationEvents = false;
                // LOCAL
                foreach (var i in Game.PlayerList)
                i.ResetPreviewSelection();
                foreach (var i in Game.PlayerList)
                i.LocalEventSystem.sendNavigationEvents = true;
            break;

            case EInputMode.Menu:
                foreach (var i in Game.PlayerList)
                i.SwitchLocalInputMode(InputMode);
                // GLOBAL
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.sendNavigationEvents = true;
                // LOCAL
                foreach (var i in Game.PlayerList)
                i.LocalEventSystem.SetSelectedGameObject(null);
                foreach (var i in Game.PlayerList)
                i.LocalEventSystem.sendNavigationEvents = false;

            break;
        }
        OnComplete?.Invoke();
    }

//  PLAYER HUD
    public void InitHUD()
    {
        var i = new ConstraintSource();
        i.sourceTransform = Game.Camera.transform;
        i.weight = 1f;

        HudConstraint.AddSource(i);
        HudConstraint.constraintActive = true;
        DisableHUD();
    }
    public void EnableHUD()
    {
        HudRoot.SetActive(true);
    }
    public void DisableHUD()
    {
        HudRoot.SetActive(false);
    }
    public void UpdateHealthbar()
    {
        HealthBar.fillAmount = HealthFactor;
    }

//  COLLISION LOGIC
    public static void IgnoreCollision(Collider collider)
    {
        foreach(Tank tank in Game.PlayerList)
        {
            foreach(var i in tank.GetComponentsInChildren<Collider>(true))
            {
                Physics.IgnoreCollision(collider, i);
            }
        }
    }
}
