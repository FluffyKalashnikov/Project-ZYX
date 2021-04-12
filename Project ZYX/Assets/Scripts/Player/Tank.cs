using System.Collections;
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
             : m_TankAsset = Game.TankTypes[TankIndex]; 
        }
        set 
        { 
            if (m_TankAsset == value) return;
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
            m_Model = Instantiate(value, PlayerTransform);
            m_Model.transform.localPosition = TankRef.ModelOffset;
            m_Model.name = "Tank Model";
            UpdateTank();
        }
    }
    public TankRef    TankRef
    {
        get 
        {
            return Model.GetComponent<TankRef>();
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

    public AudioEvent AudioIdle
    {
        get { return TankAsset ? TankAsset.AudioIdle : null; }
    }
    public AudioEvent AudioStartup
    {
        get { return TankAsset ? TankAsset.AudioStartup : null; }
    }
    public AudioEvent AudioThrottle
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
    public float      Health
    {
        get { return m_Health; }
        set { m_Health = Mathf.Clamp(value, 0, MaxHealth); UpdateHealthbar(); }
    }
    public float      HealthFactor
    {
        get {return Health/MaxHealth;}
    }
    public string     Name 
    {
        get 
        {
            if (m_Name == string.Empty)
            Name = $"Player {PlayerIndex+1}";
            return m_Name;
        }
        set 
        {
            m_Name = value; 
            Game.UpdateScoreboards();
        }
    }
    public float      Score
    {
        get { return m_Score; }
        set { m_Score = value; Game.UpdateScoreboards(); }
    }
    public int        PlayerIndex 
    {
        get {return PlayerInput.playerIndex;}
    }
    public int        TankIndex
    {
        get { return Mathf.Clamp(m_TankIndex, 0, Game.TankTypes.Length-1);  }
        set 
        { 
            // SET INDEX/UPDATE TANK
            m_TankIndex = Mathf.Clamp(value, 0, Game.TankTypes.Length-1); 
            TankAsset = Game.TankTypes.Length != 0 ? Game.TankTypes[m_TankIndex] : null;
        }
    }
    public Vector3    Position
    {
        get { return PlayerTransform.position; }
        set { PlayerTransform.position = value; }
    }
    public Quaternion Rotation
    {
        get { return PlayerTransform.rotation; }
        set { PlayerTransform.rotation = value; }
    }
    public Transform  PlayerTransform
    {
        get { return Controller.transform; }
    }

    private GameObject m_Model     = null;
    private TankAsset  m_TankAsset = null;
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
    public Animator               Animator           = null;
    public CharacterController    Controller         = null;
    public PlayerInput            PlayerInput        = null; 
    public MultiplayerEventSystem LocalEventSystem   = null;
    [Space(10)]
    public GameObject             PreviewPrefab      = null;
    public GameObject             PreviewInstance    = null;
    public GameObject             ScorePrefab        = null;
    [Space(5)]
    public Button                 PreviewButtonReady = null;
    public Button                 PreviewButtonRight = null;
    public Button                 PreviewButtonLeft  = null;
    public TMP_InputField         PreviewNameField   = null;
    public RawImage               PreviewTankImage   = null;
    public Image                  PreviewBackground  = null;
    public ParentConstraint       HudConstraint      = null;
    public GameObject             HudRoot            = null;
    public Image                  HealthBar          = null;
    #endregion

    IEnumerator IE_ResetSelect = null;

    public static Action<Tank>       OnTankFire;
    public static Action<DamageInfo> OnTankDead;

    public Action<float> Tick;

    public int Power = 0;
    public float PowerUpTimer;

    public enum EInputMode
    {
        Game,
        Lobby,
        Menu
    }

    private void Awake()
    {
        // 1. REFERENCES
        this.TankMovement     = GetComponent<TankMovement>();
        this.TankShoot        = GetComponent<TankShoot>();
        this.TankTurret       = GetComponent<TankTurret>();
        this.TankAudio        = GetComponent<TankAudio>();
        this.LocalEventSystem = GetComponent<MultiplayerEventSystem>();

        // 2. INIT LOGIC
        InitPlayer();
        UpdateTank();
        DisableTank();
        DisableTank();
        InitPreview();
        InitHUD();
        InitScore();
        

        // 3. EVENT SUBSCRIPTION
        Game.OnNewLobby += EnablePreview;
        Game.OnEndLobby += DisablePreview;
        Game.OnNewMatch += () => Ready = false;

        OnTankFire += tank => 
        { 
            if (tank != this) return;
            Animator?.Play("Shoot", 1);
            Cam.Shake(15f, 5f, 1f);
        };

        // 4. INPUT SETUP
        InputAction PauseAction  = PlayerInput.actions.FindAction("Pause");
        InputAction ResumeAction = PlayerInput.actions.FindAction("Resume");
        PauseAction .performed += ctx => 
        { 
            if (Game.IsPlaying())
            {
                Debug.Log($"PAUSED: {Game.State}");
                Game.PauseGame();
            }
        };
        ResumeAction.performed += ctx => 
        { 
            if (Game.IsPaused())
            {
                Debug.Log($"RESUME: {Game.State}");
                Game.ResumeGame();
            }
        };
    }


//  TANK SETUP
    public void UpdateTank()
    {
        UpdateReferences();
        UpdateColor();
    }
    public void UpdateReferences()
    {
        Animator = Model.GetComponent<Animator>();
    }
    public void UpdateColor()
    {
        foreach (var rd in Model.GetComponentsInChildren<MeshRenderer>(true))
        {
            foreach(var i in rd.materials)
            i.color = Color;
        }
    }
    
//  TANK HEALTH
    public void TakeDamage(DamageInfo DamageInfo)
    {
        if (Game.IsPlaying())
        if ((Health -= DamageInfo.Damage) <= 0f)
        {
            Die(DamageInfo);
        }
    }
    public void Die(DamageInfo damageInfo)
    {
        DisableTank();
        Game.OnTankKill((Tank) damageInfo.Reciever, damageInfo);
    }
    
//  TANK ENABLING
    public void EnableTank()
    {
        if (Alive) return;

        Game.AliveList.Add(this);
        Game.CameraTargets.AddMember(PlayerTransform, 1f, 0f);
        ShowTank();
        EnableHUD();
        EnableInput();

        Alive = true;
    }
    public void DisableTank()
    {
        if (!Alive) return;

        Game.AliveList.Remove(this);
        Game.CameraTargets.RemoveMember(PlayerTransform);
        HideTank();
        DisableHUD();
        DisableInput();

        Alive = false;
    }

    public void EnableInput()
    {
        EnableMove();
        EnableLook();
        EnableFire();
    }
    public void EnableMove()
    {
        TankMovement.Enable();
    }
    public void EnableLook()
    {
        TankTurret.Enable();
    }
    public void EnableFire()
    {
        TankShoot.Enable();
    }
    
    public void DisableInput()
    {
        DisableMove();
        DisableLook();
        DisableFire();
    }
    public void DisableMove()
    {
        TankMovement.Disable();
    }
    public void DisableLook()
    {
        TankTurret.Disable();
    }
    public void DisableFire()
    {
        TankShoot.Disable();
    }

    public void HideTank()
    {
        foreach (var i in Model.GetComponentsInChildren<MeshRenderer>(true))
        i.forceRenderingOff = true;
        foreach (var i in Model.GetComponentsInChildren<Light>(true))
        i.enabled = false;
    }
    public void ShowTank()
    {
        foreach (var i in Model.GetComponentsInChildren<MeshRenderer>(true))
        i.forceRenderingOff = false;
        foreach (var i in Model.GetComponentsInChildren<Light>(true))
        i.enabled = true;
    }

    public void EnableLookOnly()
    {
        DisableMove();
        EnableLook();
        DisableFire();
    }

//  TANK STATS
    public void LoadStats(TankAsset Asset)
    {
        Model = Asset.Model;
        UpdateTank();
        BroadcastMessage
        (
            "OnLoadStats", 
            TankRef, 
            SendMessageOptions.RequireReceiver
        );
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
        if (Game.State == Game.EState.Lobby)
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
        // 1. CREATE ELEMENT
        foreach (var scoreboard in Game.Instance.Scoreboards)
        {
            ScoreWidget i = Instantiate
            (
                Refs.i.ScoreboardElement, 
                scoreboard.transform
            ).GetComponent<ScoreWidget>();

            // 2. INIT AND GET REF
            scoreboard.AddWidget(i);
            i.Init(this);
        }
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
    public static void SwitchInputMode(EInputMode InputMode)
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

//  DEBUG
    #if UNITY_EDITOR
    [ContextMenu("Add Score")]
    private void DEBUG_AddScore()
    {
        Score++;
    }
    [ContextMenu("Add Health")]
    private void DEBUG_AddHealth()
    {
        Health += 20;
    }
    [ContextMenu("Take Damage")]
    private void DEBUG_TakeDamage()
    {
        DamageInfo i = new DamageInfo
        (
            20f,
            DamageType.OutOfBounds,
            this,
            this
        );
        TakeDamage(i);
    }
    #endif
}
