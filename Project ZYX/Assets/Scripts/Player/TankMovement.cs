using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TankMovement : MonoBehaviour
{
    #region Setup Var
    ActionAsset actionAsset;
    [SerializeField] private Tank tankScript;

    public bool moveable = false;
    #endregion

    #region Stats
    [Header("Force Values")]
    [SerializeField] private float motorForce;
    [SerializeField] private float accelerationForce;
    [SerializeField] private float decelerationForce;
    [SerializeField] private float rotationForce;
    #endregion

    #region Audio
    [Header("Audio")]
    [SerializeField] private AudioEvent engineStartup;
    [SerializeField] private AudioEvent engineIdle;
    [SerializeField] private AudioEvent engineThrottle;
    [SerializeField] private AudioEvent[] engineRev;
    [SerializeField] private AudioSource engineStartupSource;
    [SerializeField] private AudioSource engineIdleSource;
    [SerializeField] private AudioSource engineThrottleSource;
    [SerializeField] private AudioSource engineRevSource;

    [SerializeField] float velocityMax;
    [SerializeField] float idleVolumeMin;
    [SerializeField] float idleVolumeMax;
    [SerializeField] float throttlePitchMin;
    [SerializeField] float throttlePitchMax;
    [SerializeField] float throttleVolumeMin;
    [SerializeField] float throttleVolumeMax;

    private float currentVelMultiplier;
    public float Timer;
    #endregion

    #region Vectors
    private Vector3 driveForce;
    private Vector3 currentVel;
    private Vector3 direction;
    #endregion

    #region Audio Bools
    private bool ifReving = false;
    #endregion



    #region Setup
    private void Awake()
    {
        actionAsset = new ActionAsset();
        tankScript = GetComponent<Tank>();
    }
    void Start()
    {
        StartCoroutine(EngineStartUpSound());
        actionAsset.Player.Enable();
    }
    private void Update()
    {
        if (moveable)
        {
            BaseMovement(actionAsset.Player.Move.ReadValue<Vector2>());
            EngineRev(actionAsset.Player.Move.ReadValue<Vector2>());
        }
        VolumeManager();

        if (Timer < 0)
        {
            Timer -= Time.deltaTime;
            accelerationForce = 0.2f;
        }
        else if (Timer > 0)
        {
            Timer -= Time.deltaTime;
            accelerationForce = 0.6f;
        }
    }
    #endregion

    #region Movement Functions
    private void BaseMovement(Vector2 input)
    {
        #region Actual Movement
        float multipliedMotorForce = input.y * motorForce;
        driveForce = new Vector3(0, 0, multipliedMotorForce) * Time.deltaTime;
        #region Accel/Decel-Physics
        if (input.y != 0)
        {
            currentVel = Vector3.Lerp(currentVel, driveForce, accelerationForce * Time.deltaTime);
        }
        else if (input.y == 0 && currentVel.z != 0)
        {
            currentVel = Vector3.Lerp(currentVel, Vector3.zero, decelerationForce * Time.deltaTime);
        }
        #endregion
        float multipliedRotationForce = input.x * rotationForce;

        tankScript.Controller.transform.Rotate(0, multipliedRotationForce * Time.deltaTime, 0);
        direction = tankScript.Controller.transform.TransformDirection(currentVel);
        Debug.Log(currentVel.z);
        tankScript.Controller.Move(direction);
        #endregion
    }
    #endregion

    #region EngineRev
    private void EngineRev(Vector2 input)
    {
        if (engineRev != null && input.y > 0 && input.y <= 0.3 && !ifReving || engineRev != null && input.y < 0 && input.y >= -0.3 && !ifReving)
        {
            engineRevSource.Stop();
            engineRev[0].Play(engineRevSource);
            ifReving = true;
        }
        else if (input.y == 0 && ifReving)
        {
            ifReving = false;
        }

        if (engineRev != null && input.y > 0.3 && input.y <= 0.6 && !ifReving || engineRev != null && input.y < -0.3 && input.y >= -0.6 && !ifReving)
        {
            engineRevSource.Stop();
            engineRev[1].Play(engineRevSource);
            ifReving = true;
        }
        else if (input.y == 0 && ifReving)
        {
            ifReving = false;
        }

        if (engineRev != null && input.y > 0.6 && !ifReving || engineRev != null && input.y < -0.6 && !ifReving)
        {
            engineRevSource.Stop();
            engineRev[2].Play(engineRevSource);
            ifReving = true;
        }
        else if (input.y == 0 && ifReving)
        {
            ifReving = false;
        }
    }
    #endregion

    #region Audio Manager
    private void VolumeManager()
    {
        //Absolutes currentVel
        float tankVelAbs = Mathf.Abs(currentVel.z);
        float velocityScale = tankVelAbs / velocityMax;

        engineIdleSource.volume = Mathf.Lerp(idleVolumeMax, idleVolumeMin, velocityScale);
        engineThrottleSource.pitch = Mathf.Lerp(throttlePitchMin, throttlePitchMax, velocityScale);
        engineThrottleSource.volume = Mathf.Lerp(throttleVolumeMin, throttleVolumeMax, velocityScale);
    }
    IEnumerator EngineStartUpSound()
    {
        engineStartup.Play(engineStartupSource);

        yield return new WaitForSeconds(1.1568f);

        engineIdle.Play(engineIdleSource);
        engineThrottle.Play(engineThrottleSource);
    }
    #endregion

    public void enableTankMovement()
    {
        moveable = true;
    }
}
