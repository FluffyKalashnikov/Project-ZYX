using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour
{
    [Header("Name Settings")]
    [SerializeField] private int maxNameLength = 13;

    private bool ready = false;
    private IEnumerator IE_ResetSelect = null;

    [Header("REFERENCES")]
    [SerializeField] private Tank          owner          = null;
    [SerializeField] private GameObject    previewPrefab  = null;
    [Space(10)]
    [SerializeField] private GameObject    previewRoot    = null;
    [SerializeField] private Image         previewBackrnd = null;
    [SerializeField] private GameObject    previewObject  = null;
    [SerializeField] private RenderTexture previewTexture = null;
    [SerializeField] private Camera        previewCamera  = null;
    [SerializeField] private RawImage      previewImage   = null;
    [SerializeField] private GameObject    previewTank    = null;
    [Space(10)]
    [SerializeField] private Button         buttonRDY = null;
    [SerializeField] private Button         buttonRGT = null;
    [SerializeField] private Button         buttonLFT = null;
    [SerializeField] private TMP_InputField nameField = null;
    [Space(10)]
    [SerializeField] private MultiplayerEventSystem eventSystem = null;



    private void Start() 
    {
        // 1. CREATE USER INTERFACE
        previewRoot   = Game.Instance.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
        previewObject = Instantiate(previewPrefab, previewRoot.transform);
        eventSystem.playerRoot = previewObject;

        // 2. GET REFERENCES
        Button[] buttons = previewObject.GetComponentsInChildren<Button>(true);
        buttonRDY = buttons[0];
        buttonRGT = buttons[1];
        buttonLFT = buttons[2];

        previewImage   = previewObject.GetComponentInChildren<RawImage>();
        previewBackrnd = previewObject.GetComponentInChildren<Image>();
        nameField      = previewObject.GetComponentInChildren<TMP_InputField>();

        // 3. CHECK REFERENCES
        #if UNITY_EDITOR
        if (!previewPrefab) Debug.LogError("Preview Prefab has not been assigned.", this);
        if (!owner)         Debug.LogError("Tank Owner has not been assigned.",     this);
        if (!previewRoot)   Debug.LogError("Preview Root has not been assigned.",   this);
        if (!previewCamera) Debug.LogError("Preview Camera has not been assigned.", this);
        if (!previewImage)  Debug.LogError("Preview Image has not been assigned.",  this);
        if (!previewTank)   Debug.LogError("Preview Tank has not been assigned.",   this);
        if (!buttonRDY)     Debug.LogError("Ready Buttonhas not been assigned.",    this);
        if (!buttonRGT)     Debug.LogError("Right Button has not been assigned.",   this);
        if (!buttonLFT)     Debug.LogError("Left Button has not been assigned.",    this);
        if (!eventSystem)   Debug.LogError("Event System has not been assigned.",   this);
        if (!nameField)     Debug.LogError("Name Field has not been assigned.",     this);
        #endif


        // 4. CREATE/ASSIGN PREVIEW TEXTURE
        previewTexture = new RenderTexture(256, 256, 24);
        previewCamera.targetTexture = previewTexture;
        previewImage .texture       = previewTexture;

        // 5. SUBSCRIBE TO EVENTS
        Game.OnStartLobby += EnablePreview;
        Game.OnStartMatch += DisablePreview;
        Game.OnStartMatch += () => ready = false;

        nameField.onValidateInput += NameValidation;
        nameField.onSubmit.AddListener(NameSubmit);
        nameField.onSelect.AddListener(NameSelect);

        buttonRDY.onClick .AddListener
        (
            () =>
            {
                switch (ready = !ready)
                {
                    case true:  Ready();   break;
                    case false: Unready(); break;
                }
            }
        );
        
        // 4. INITIALIZE
        DisablePreview();
        previewBackrnd.color = Game.PlayerColors[owner.PlayerInput.playerIndex];
    }
    


    // NAME INPUT FIELD
    private char NameValidation(string text, int charIndex, char addedChar)
    {
        // 1. RETURN NULL IF TOO LONG
        if (text.Length >= maxNameLength)
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
        owner.name = text.Trim();

        // 2. UNSELECT INPUT FIELD
        nameField.SetTextWithoutNotify(owner.name);
        nameField.DeactivateInputField();
        nameField.caretWidth = 0;
        ResetPreviewSelection();
    }
    private void NameSelect(string text)
    {
        nameField.caretWidth = 1;
    }
    
    // USER INTERFACE
    public void ResetPreviewSelection()
    {
        if (IE_ResetSelect != null)
        StopCoroutine (IE_ResetSelect);
        StartCoroutine(IE_ResetSelect = Logic());

        IEnumerator Logic()
        {
            yield return new WaitUntil(() => buttonRDY.enabled || buttonRDY != null);
            eventSystem.        SetSelectedGameObject(buttonRDY.gameObject);
            EventSystem.current.SetSelectedGameObject(buttonRDY.gameObject);
            nameField.DeactivateInputField(false);
        }
    }
    public void EnablePreview()
    {
        previewCamera.enabled = true;
        previewRoot.SetActive(true);
        previewTank.SetActive(true);
        previewTexture.Create();

        ResetPreviewSelection();
    }
    public void DisablePreview()
    {
        previewCamera.enabled = false;
        previewRoot.SetActive(false);
        previewTank.SetActive(false);
        previewTexture.Release();
    }

    // LOBBY LOGIC
    private void Ready()
    {
        Debug.Log("Ready!");
        Game.ReadyCount++;
        Game.OnPlayerReady(owner);
    }
    private void Unready()
    {
        Debug.Log("Unready...");
        Game.ReadyCount--;
        Game.OnPlayerReady(owner);
    }
}
