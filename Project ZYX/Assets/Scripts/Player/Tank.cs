﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(MultiplayerEventSystem), typeof(InputSystemUIInputModule), typeof(PlayerInput))]
public class Tank : MonoBehaviour, IDamageable
{
    [Header("Variables")]
    public new string name = "Player 1";
    public float maxHealth = 100f;
    public float health;
    public bool  alive = false;
    public bool  Ready = false;
    public Color Color 
    {
        get
        {
            return Game.Instance.PlayerColors[PlayerInput.playerIndex];
        }
    }
    
    [Header("REFERENCES")]
    public TankMovement           TankMovement  = null;
    public TankShoot              TankShoot     = null;
    public TankTurret             TankTurret    = null;
    public CharacterController    Controller    = null;
    public PlayerInput            PlayerInput   = null; 
    public MultiplayerEventSystem EventSystem   = null;
    public MeshRenderer[]         TankRenderers = null;

    [SerializeField]
    private Image[] imagesToColor = null;

    public static       Action<Tank> OnTankFire;
    public static event Action<Tank> OnTankDeath = tank => tank.Die();
    public static       MultiplayerEventSystem MainEventSystem = null;

    [Header("Unity Events")]
    public UnityEvent OnEnable;
    public UnityEvent OnDisable;


    public int Power = 0;
    public float PowerUpTimer;

    public void Awake()
    {
        // 1. REFERENCES
        if (!MainEventSystem) MainEventSystem = GetComponent<MultiplayerEventSystem>();

        this.TankMovement  = GetComponent<TankMovement>();
        this.TankShoot     = GetComponent<TankShoot>();
        this.TankTurret    = GetComponent<TankTurret>();
        this.EventSystem   = GetComponent<MultiplayerEventSystem>();
        this.TankRenderers = GetComponentsInChildren<MeshRenderer>(true);

        // 2. SETUP
        gameObject.name = name = $"Player {PlayerInput.playerIndex+1}";
        health = maxHealth;

        // 3. UPDATE STATS
        UpdateTank();
        
        // 4. GAMEMODE LOGIC
        Game.PlayerList.Add(this);
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
        if ((health = Mathf.Clamp(health - damage, 0f, maxHealth)) <= 0f)
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
            Disable();
        }
    }

//  TANK STATE
    public void Enable()
    {
        if (alive) return;

        Game.AliveList.Add(this);
        Game.CameraTargets.AddMember(Controller.transform, 1f, 0f);
        ShowTank();

        OnEnable.Invoke();
        alive = true;
    }
    public void Disable()
    {
        if (!alive) return;

        Game.AliveList.Remove(this);
        Game.CameraTargets.RemoveMember(Controller.transform);
        HideTank();

        OnDisable.Invoke();
        alive = false;
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
}
