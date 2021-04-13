using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Tank), typeof(PlayerInput))]
public class TankShoot : MonoBehaviour
{
    #region knas
    private Tank TankScript
    {
        get { return m_TankScript ? m_TankScript : m_TankScript = GetComponent<Tank>(); }
        set { m_TankScript = value; }
    }
    private Tank m_TankScript = null;
    #endregion

    [Header("Script")]
    [SerializeField] private TankAudio tankAudio;

    [Header("Fire Event")]
    [SerializeField] private FireEventSingleMulti fireEvent;
    private FireEventSingleMulti ChargedFireEvent;


    [Header("Firing Stats")]
    [SerializeField] private float bulletVelocity = 25f;
    public float minCharge      = 0.3f;
    public float maxCharge      = 1f;
    [SerializeField] private float chargeTime     = 1f;
    [SerializeField] private float inputTolerance = 0.5f;
    [SerializeField] private float fireRate       = 1f;

    [Header("References")]
    [SerializeField] private Transform  MuzzlePoint;

    // PRIVATE VARIABLES
    private float       FireTimestamp = 0;

    // PRIVATE REFERENCES
    //private Tank        TankScript    = null;
    private PlayerInput PlayerInput   = null;
    private InputAction FireAction    = new InputAction();
    private IEnumerator IE_Fire       = null;

    private void Awake()
    {
        // 1. GET REFERENCES
        TankScript  = GetComponent<Tank>();
        PlayerInput = GetComponent<PlayerInput>();
        
        FireAction = PlayerInput.actions.FindAction("Fire", true);

        // 2. EVENT SUBSCRIPTION
        FireAction.canceled += Fire;

        Game.OnPauseReset += () => 
        {
            if (IE_Fire != null) 
            StopCoroutine(IE_Fire);
        };
    }

    private void Fire(float charge)
    {
        switch (charge > 0.95)
        {
            case true:
                ChargedFireEvent.Fire(TankScript, MuzzlePoint, charge, bulletVelocity);
                break;
            default:
                fireEvent.Fire(TankScript, MuzzlePoint, charge, bulletVelocity);
                break;
        }
        #region audio
        bool audioPlayed = false;
        if (charge > 0.95 && !audioPlayed)
        {
            tankAudio.ChargeAbility();
            audioPlayed = true;
        }
        else
        {
            audioPlayed = false;
        }
        #endregion





        #region OLD
        /*if(tankPowerupsScript.Multishot_Enabled)
        {
            for (int i = 0; i < tankPowerupsScript.Multishot_Ammount; i++)
            {
                Quaternion target = Quaternion.AngleAxis(tankPowerupsScript.Multishot_Angle * (i - (tankPowerupsScript.Multishot_Ammount / 2)) - -1 * (tankPowerupsScript.Multishot_Angle / 2), transform.up);
                // 1. CREATE BULLET
                var Shell = Instantiate
                (
                    ShellPrefab,
                    MuzzlePoint.position,
                    target * MuzzlePoint.rotation
                ).GetComponent<Shell>();

                // 2. INIT BULLET
                Shell.Init(bulletVelocity * charge, TankScript);
                Tank.OnTankFire.Invoke(TankScript);
            }
        }

        else if (!tankPowerupsScript.Multishot_Enabled)
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
        }*/
        #endregion
    }
    private void Fire(InputAction.CallbackContext ctx)
    {
        if (IE_Fire != null)
        MatchContext.Remove(IE_Fire);
        MatchContext.Add(IE_Fire = Logic());

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

    public void Enable()
    {
        FireAction.Enable();
    }
    public void Disable()
    {
        FireAction.Disable();
    }

    public void OnLoadStats(TankRef i)
    {
        MuzzlePoint = i.MuzzlePoint;
        ChargedFireEvent = TankScript.TankAsset.FireChargeAbility;
    }
}
