using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankPowerups : MonoBehaviour
{
    InputAction seaminePlacementAction;
    PlayerInput playerInput;

    #region Stats
    [Header("Scripts")]
    [SerializeField] private Tank tankScript;
    [SerializeField] private TankMovement tankMovementScript;
    [SerializeField] private TankShoot tankShootScript;
    [SerializeField] private TankAudio tankAudioScript;

    [Header("Events")]
    [SerializeField] private DestructEvent pickupDestroyer;
    [SerializeField] private FireEventSingleMulti fireEvent;

    [Header("Seamine")]
    [SerializeField] private List<GameObject> SeamineList = new List<GameObject>();
    [SerializeField] private Transform SeaminePoint;
    [SerializeField] private GameObject seamineObject;
    private bool seamineActive;

    [Header("SpeedBoost")]
    [SerializeField] private float SpeedBoost_AudioMultiplier;

    [Header("Bouncy Bullets")]
    [SerializeField] private GameObject BouncyBulletPrefab;

    [Header("PowerUp Ammounts")]
    [SerializeField] private float HP_Ammount;
    [SerializeField] private int Seamine_Ammount;

    [Header("PowerUp durations")]
    [SerializeField] private float BouncyBullets_Duration;
    [SerializeField] private float QuickCharge_Duration;
    [SerializeField] private float Multishot_Duration;
    [SerializeField] private float SpeedBoost_Duration;
    [SerializeField] private float Invincibility_Duration;

    [Header("PowerUp Multipliers")]
    [SerializeField] private float SpeedBoost_Multiplier;
    #endregion

    #region Private variables
    //[SPEED_BOOST] Powerup bools
    private bool SpeedBoost_Picked = false;
    private bool SpeedBoost_TimerBool = false;

    //[SPEED_BOOST] Backup variables
    private float currentMotorforce;
    private float currentMaxVel;
    private float currentThrottlePitch;
    private float currentSpeedBoostDuration;

    //[QUICK_CHARGE] Powerup bools
    private bool QuickCharge_Picked = false;
    private bool QuickCharge_TimerBool = false;

    //[QUICK_CHARGE] Backup variables
    private float currentMinCharge;
    private float currentQuickChargeDuration;

    //[MULTISHOT] Powerup bools
    [HideInInspector]
    public bool Multishot_Enabled = false;
    private bool Multishot_Picked = false;
    private bool Multishot_TimerBool = false;

    //[MULTISHOT] Backup variables
    private float currentMultishotDuration;

    //[BOUNCY_BULLETS] Powerup bool
    private bool BouncyBullets_Picked = false;
    private bool BouncyBullets_TimerBool = false;

    //[QUICK_CHARGE] Backup variables
    private GameObject standardBullet;
    private float currentBouncyBulletsDuration;
    #endregion

    private void Awake()
    {
        tankScript = GetComponent<Tank>();
        playerInput = GetComponent<PlayerInput>();

        seaminePlacementAction = playerInput.actions.FindAction("Seamine");
        seaminePlacementAction.started += _ => SeamineOperator();
    }
    private void Start()
    {
        seamineActive = false;
    }
    private void Update()
    {
        SpeedBoostTimerMethod();
        MultishotTimerMethod();
        QuickChargeTimerMethod();
        BouncyBulletsTimerMethod();
        tankAudioScript.PickupSpeedBoostLOOPSound();
    }

    private void OnTriggerEnter(Collider powerup)
    {
        switch (powerup.gameObject.tag)
        {
            case "PU-HP":
                if(tankScript.Health != tankScript.MaxHealth)
                {
                    HPMethod();
                    pickupDestroyer.Play(powerup.gameObject);
                }
                else
                {
                    return;
                }
                break;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            case "PU-Seamine":
                if (SeamineList.Count != Seamine_Ammount)
                {
                    SeamineMethod();
                    pickupDestroyer.Play(powerup.gameObject);
                }
                else
                {
                    return;
                }
                break;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //In this case, it's actually for bouncy bullets, since it replaced the EMP Pickup
            case "PU-EMP":
                if (BouncyBullets_Picked == false)
                {
                    BouncyBullets_Picked = true;
                    BouncyBulletsMethod();
                    pickupDestroyer.Play(powerup.gameObject);
                }
                else
                {
                    return;
                }
                break;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            case "PU-QuickCharge":
                if (QuickCharge_Picked == false)
                {
                    QuickCharge_Picked = true;
                    QuickChargeMethod();
                    pickupDestroyer.Play(powerup.gameObject);
                }
                else
                {
                    return;
                }
                break;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            case "PU-Multishot":
                if (Multishot_Picked == false)
                {
                    Multishot_Picked = true;
                    MultishotMethod();
                    pickupDestroyer.Play(powerup.gameObject);
                }
                else
                {
                    return;
                }
                break;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            case "PU-SpeedBoost":
                if(SpeedBoost_Picked == false)
                {
                    SpeedBoost_Picked = true;
                    SpeedBoostMethod();
                    pickupDestroyer.Play(powerup.gameObject);
                }
                else
                {
                    return;
                }
                break;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            case "PU-Invincibility":
                InvincibilityMethod();
                break;
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    #region Pickup Applier
    private void HPMethod()
    {
        tankScript.Health += HP_Ammount;
        tankAudioScript.PickupHPSound();
    }
    private void SeamineMethod()
    {
        SeamineList.Clear();
        for (int i = 0; i < Seamine_Ammount; ++i)
        {
            SeamineList.Add(seamineObject);
        }
        seamineActive = true;
    }
    private void BouncyBulletsMethod()
    {
        tankAudioScript.PickupBBSound();
        currentBouncyBulletsDuration = BouncyBullets_Duration;
        if (currentBouncyBulletsDuration != 0)
        {
            standardBullet = fireEvent.ShellPrefab;
            fireEvent.ShellPrefab = BouncyBulletPrefab;
            Debug.Log(fireEvent.ShellPrefab);
        }
    }
    private void QuickChargeMethod()
    {
        tankAudioScript.PickupQCSound();
        currentQuickChargeDuration = QuickCharge_Duration;
        if (currentQuickChargeDuration != 0)
        {
            currentMinCharge = tankShootScript.minCharge;
            tankShootScript.minCharge = tankShootScript.maxCharge;
        }
    }
    private void MultishotMethod()
    {
        tankAudioScript.PickupMSSound();
        currentMultishotDuration = Multishot_Duration;
        if (currentMultishotDuration != 0)
        {
            Multishot_Enabled = true;
        }
    }
    private void SpeedBoostMethod()
    {
        tankAudioScript.PickupSpeedBoostSTARTSound();
        currentSpeedBoostDuration = SpeedBoost_Duration;
        if (currentSpeedBoostDuration != 0)
        {
            currentMotorforce = tankMovementScript.motorForce;
            tankMovementScript.motorForce *= SpeedBoost_Multiplier;

            currentMaxVel = tankMovementScript.velocityMax;
            tankMovementScript.velocityMax *= SpeedBoost_Multiplier;

            currentThrottlePitch = tankAudioScript.throttlePitchMax;
            tankAudioScript.throttlePitchMax *= SpeedBoost_AudioMultiplier;
        }
    }
    private void InvincibilityMethod()
    {
        Debug.Log("invincible");
    }
    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    #region Pickup Operators
    private void SeamineOperator()
    {
        if (SeamineList.Count >= 1 && seamineActive == true)
        {
            var SemineExplosive = Instantiate(SeamineList[0], new Vector3(SeaminePoint.position.x, SeaminePoint.position.y, SeaminePoint.position.z), Quaternion.identity).GetComponent<SeamineExplosive>();
            SemineExplosive.Init(tankScript);
            SeamineList.RemoveAt(0);
        }
    }
    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    #region Pickup Timers
    private void SpeedBoostTimerMethod()
    {
        if (SpeedBoost_Picked == true)
        {
            if (SpeedBoost_TimerBool == false && currentSpeedBoostDuration > 0)
            {
                StartCoroutine(SpeedBoostTimer());
            }
            else if (currentSpeedBoostDuration == 0)
            {
                tankMovementScript.motorForce = currentMotorforce;
                tankMovementScript.velocityMax = currentMaxVel;
                tankAudioScript.throttlePitchMax = currentThrottlePitch;
                SpeedBoost_Picked = false;
                tankAudioScript.PickupSpeedBoostENDSound();
            }
        }
    }
    private void MultishotTimerMethod()
    {
        if (Multishot_Picked == true)
        {
            if (Multishot_TimerBool == false && currentMultishotDuration > 0)
            {
                StartCoroutine(MultishotTimer());
            }
            else if (currentMultishotDuration == 0)
            {
                Multishot_Enabled = false;
                Multishot_Picked = false;
            }
        }
    }
    private void QuickChargeTimerMethod()
    {
        if (QuickCharge_Picked == true)
        {
            if (QuickCharge_TimerBool == false && currentQuickChargeDuration > 0)
            {
                StartCoroutine(QuickChargeTimer());
            }
            else if (currentQuickChargeDuration == 0)
            {
                tankShootScript.minCharge = currentMinCharge;
                QuickCharge_Picked = false;
            }
        }
    }
    private void BouncyBulletsTimerMethod()
    {
        if (BouncyBullets_Picked == true)
        {
            if (BouncyBullets_TimerBool == false && currentBouncyBulletsDuration > 0)
            {
                StartCoroutine(BouncyBulletsTimer());
            }
            else if (currentBouncyBulletsDuration == 0)
            {
                fireEvent.ShellPrefab = standardBullet;
                BouncyBullets_Picked = false;
            }
        }
    }
    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    #region Pickup Timers COROUTINES
    IEnumerator SpeedBoostTimer()
    {
        SpeedBoost_TimerBool = true;
        yield return new WaitForSeconds(1);
        currentSpeedBoostDuration -= 1;

        //Set bool to false
        SpeedBoost_TimerBool = false;
    }
    IEnumerator QuickChargeTimer()
    {
        QuickCharge_TimerBool = true;
        yield return new WaitForSeconds(1);
        currentQuickChargeDuration -= 1;

        //Set bool to false
        QuickCharge_TimerBool = false;
    }
    IEnumerator BouncyBulletsTimer()
    {
        BouncyBullets_TimerBool = true;
        yield return new WaitForSeconds(1);
        currentBouncyBulletsDuration -= 1;

        //Set bool to false
        BouncyBullets_TimerBool = false;
    }
    IEnumerator MultishotTimer()
    {
        Multishot_TimerBool = true;
        yield return new WaitForSeconds(1);
        currentMultishotDuration -= 1;

        //Set bool to false
        Multishot_TimerBool = false;
    }
    #endregion










    public void OnLoadStats(TankRef i)
    {
        SeaminePoint = i.SeaminePoint;
    }
}