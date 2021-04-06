using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Tank), typeof(PlayerInput))]
public class TankShoot : MonoBehaviour
{
    [Header("Firing Stats")]
    [SerializeField] private float bulletVelocity = 25f;
    public float minCharge      = 0.3f;
    public float maxCharge      = 1f;
    [SerializeField] private float chargeTime     = 1f;
    [SerializeField] private float inputTolerance = 0.5f;
    [SerializeField] private float fireRate       = 1f;

    [Header("References")]
    [SerializeField] private Transform  MuzzlePoint;
    [SerializeField] private GameObject ShellPrefab;

    // PRIVATE VARIABLES
    private float       FireTimestamp = 0;

    // PRIVATE REFERENCES
    private Tank        TankScript    = null;
    private PlayerInput PlayerInput   = null;
    private InputAction FireAction    = null;
    private IEnumerator IE_Fire       = null;


    private void Awake()
    {
        // 1. GET REFERENCES
        TankScript  = GetComponent<Tank>();
        PlayerInput = GetComponent<PlayerInput>();
        
        FireAction = PlayerInput.actions.FindAction("Fire", true);

        // 2. EVENT SUBSCRIPTION
        FireAction.canceled += Fire;

        Tank.OnTankFire += tank => 
        {
            if (tank != TankScript) return;

            Cam.Shake(15f, 5f, 1f);
        };
        Game.OnPauseReset += () => 
        {
            if (IE_Fire != null) 
            StopCoroutine(IE_Fire);
        };
    }


    private void Fire(float charge)
    {
        // 1. CREATE BULLET
        var Shell = Instantiate
        (
            ShellPrefab, 
            MuzzlePoint.position, 
            MuzzlePoint.rotation
        ).GetComponent<Shell>();

        // 2. INIT BULLET
        Shell.Init(bulletVelocity * charge, TankScript);
        Tank.OnTankFire.Invoke(TankScript);
    }

    private void Fire(InputAction.CallbackContext ctx)
    {
        if (IE_Fire != null)
        StopCoroutine (IE_Fire);
        StartCoroutine(IE_Fire = Logic());

        IEnumerator Logic()
        {
            // 1. WAIT WHILE CAN'T FIRE
            float time = Time.time;
            yield return new WaitUntil
            (
                () => { return FireTimestamp + fireRate <= Time.time;}
            );
            if (!FireAction.enabled) yield break;
            if (time + inputTolerance < Time.time) yield break;

            // 2. FIRE ACTION
            FireTimestamp = Time.time;
            Fire
            (   
                Mathf.Lerp
                (
                    minCharge, 
                    maxCharge, 
                    Mathf.Min((float) ctx.duration/chargeTime, 1f)
                )
            );
        }
    }



    public void OnLoadStats(TankRef i)
    {
        MuzzlePoint = i.MuzzlePoint;
    }
}
