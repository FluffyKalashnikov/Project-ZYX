using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Tank), typeof(PlayerInput))]
public class TankShoot : MonoBehaviour
{
    [Header("Firing Stats")]
    [SerializeField] private float bulletVelocity = 25f;
    [SerializeField] private float minCharge      = 0.6f;
    [SerializeField] private float maxCharge      = 1f;
    [SerializeField] private float chargeTime     = 1f;

    [Header("References")]
    [SerializeField] private Transform  MuzzlePoint;
    [SerializeField] private GameObject ShellPrefab;

    // PRIVATE REFERENCES
    private Tank        TankScript  = null;
    private PlayerInput PlayerInput = null;

  


    private void Awake()
    {
        // 1. GET REFERENCES
        TankScript  = GetComponent<Tank>();
        PlayerInput = GetComponent<PlayerInput>();
        
        InputAction FireAction = PlayerInput.actions.FindAction("Fire", true);

        // 32. EVENT SUBSCRIPTION
        FireAction.started  += ctx => Debug.Log("Charging...");
        FireAction.canceled += ctx => Fire(Mathf.Lerp(minCharge, maxCharge, Mathf.Min((float) ctx.duration/chargeTime, 1f)));

        TankScript.OnTankFire += () => Cam.Shake(15f, 5f, 1f);
    }


    private void Fire(float charge)
    {
        Debug.Log($"Firing! [CHARGE: {charge}]");

        // 1. CREATE BULLET
        var Shell = Instantiate
        (
            ShellPrefab, 
            MuzzlePoint.position, 
            MuzzlePoint.rotation
        ).GetComponent<Shell>();

        // 2. INIT BULLET
        Shell.Init(bulletVelocity * charge, TankScript);
        TankScript?.OnTankFire.Invoke();
    }
}
