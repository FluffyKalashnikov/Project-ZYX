using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTurret : MonoBehaviour
{
    #region Setup Var
    ActionAsset actionAsset;
    [SerializeField] private Tank tankScript;
    [SerializeField] private TankAudio tankAudioScript;
    #endregion

    #region Audio
    /*[Header("Audio Objects")]
    [SerializeField] private AudioEvent spinStart;
    [SerializeField] private AudioSource spinStartSource;

    [SerializeField] private AudioEvent spinLoop;
    [SerializeField] private AudioSource spinLoopSource;

    [SerializeField] private AudioEvent spinStop;
    [SerializeField] private AudioSource spinStopSource;

    [SerializeField] private AudioEvent barrelLoop;
    [SerializeField] private AudioSource barrelLoopSource;

    [SerializeField] private AudioEvent barrelStuckStart;
    [SerializeField] private AudioSource barrelStuckStartSource;

    [SerializeField] private AudioEvent barrelStuckLoop;
    [SerializeField] private AudioSource barrelStuckLoopSource;*/
    #endregion

    #region Stats
    [Header("Force Values")]
    [SerializeField] private float spinForce = 50;
    [Header("Turret Objects")]
    [SerializeField] private GameObject turret;
    [SerializeField] private GameObject barrel;
    [Header("Clamp Values")]
    [SerializeField] private float gunClampMin = -100;
    [SerializeField] private float gunClampMax = -70;

    private Vector3 turretSpin;

    private float gunElevation = -90f;

    private bool spinningBool;
    private bool barrelSpinningBool;

    private bool barrelStuckBool;
    #endregion

    #region Setup
    private void Awake()
    {
        actionAsset = new ActionAsset();
        tankScript = GetComponent<Tank>();
    }
    void Start()
    {
        actionAsset.Player.Enable();
    }
    private void Update()
    {
        TurretMovement(actionAsset.Player.Turret.ReadValue<Vector2>());
        BarrelMovement(actionAsset.Player.Turret.ReadValue<Vector2>());
    }
    #endregion

    #region Spin Functions
    private void TurretMovement(Vector2 input)
    {
        float multipliedSpinForce = input.x * spinForce;
        turretSpin = new Vector3(0, multipliedSpinForce, 0) * Time.deltaTime;
        turret.transform.Rotate(turretSpin);

        #region Audio
        if (input.x != 0 && !spinningBool)
        {
            tankAudioScript.TurretSoundPlay();
            spinningBool = true;
        }
        else if (input.x == 0 && spinningBool)
        {
            tankAudioScript.TurretSoundStop();
            spinningBool = false;
        }
        #endregion
    }
    private void BarrelMovement(Vector2 input)
    {
        gunElevation += input.y * spinForce * Time.deltaTime;
        gunElevation = Mathf.Clamp(gunElevation, gunClampMin, gunClampMax);

        barrel.transform.localRotation = Quaternion.Euler(gunElevation, 0, 0);

        #region Audio
        if (input.y != 0 && !barrelSpinningBool)
        {
            tankAudioScript.BarrelSoundPlay();
            barrelSpinningBool = true;
        }
        else if (input.y == 0 && barrelSpinningBool)
        {
            tankAudioScript.BarrelSoundStop();
            barrelSpinningBool = false;
        }


        if (gunElevation == gunClampMin || gunElevation == gunClampMax)
        {
            if (input.y != 0 && !barrelStuckBool)
            {
                tankAudioScript.BarrelStuckPlay();
                barrelStuckBool = true;
            }

            else if (input.y == 0 && barrelStuckBool)
            {
                if (gunElevation != gunClampMin || gunElevation != gunClampMax)
                {
                    barrelStuckBool = false;
                }
            }
        }
        #endregion
    }
    #endregion


    public void OnLoadStats()
    {
        turret = tankScript.Model.GetComponent<TankRef>().TurretTransform.gameObject;
    }
}
