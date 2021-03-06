using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tank : MonoBehaviour, IDamageable
{
    [Header("Variables")]
    public new string name = "Player 1";
    public float maxHealth = 100f;
    public float health;
    public bool  alive = false;
    [Header("REFERENCES")]
    public CharacterController Controller      = null;
    public PlayerInput         PlayerInput     = null; 
    [SerializeField]
    private Image[] imagesToColor = null;

    public static       Action<Tank> OnTankFire;
    public static event Action<Tank> OnTankDeath = tank => tank.Die();

    [Header("Unity Events")]
    public UnityEvent OnEnable;
    public UnityEvent OnDisable;


    public int Power = 0;
    public float PowerUpTimer;

    public void Initialize()
    {
        gameObject.name = name = $"Player {PlayerInput.playerIndex+1}";
        health = maxHealth;


        foreach (var i in GetComponentsInChildren<MeshRenderer>())
        {
            i.material.color = Game.PlayerColors[PlayerInput.playerIndex];
        }
        foreach (var i in imagesToColor)
        {
            i.color = Game.PlayerColors[PlayerInput.playerIndex];
        }
    }
    
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


    public void Enable()
    {
        if (alive) return;

        Game.AliveList.Add(this);
        Game.CameraTargets.AddMember(Controller.transform, 1f, 0f);

        OnEnable.Invoke();
        alive = true;
    }
    public void Disable()
    {
        if (!alive) return;

        Game.AliveList.Remove(this);
        Game.CameraTargets.RemoveMember(Controller.transform);

        OnDisable.Invoke();
        alive = false;
    }
}
