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
    #endregion

    #region Stats
    [Header("Force Values")]
    [SerializeField] private float motorForce;
    [SerializeField] private float accelerationForce;
    [SerializeField] private float decelerationForce;
    [SerializeField] private float rotationForce;
    #endregion

    #region Audio
    [Header("Audio Objects")]
    [SerializeField] private AudioEvent engineStartup;
    [SerializeField] private AudioEvent engineIdle;
    [SerializeField] private AudioEvent engineThrottle;
    [SerializeField] private AudioEvent[] engineRev;
    [SerializeField] private AudioSource engineStartupSource;
    [SerializeField] private AudioSource engineIdleSource;
    [SerializeField] private AudioSource engineThrottleSource;
    [SerializeField] private AudioSource engineRevSource;

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
    private bool driveable = false;
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
        SFXMultiplierSetup();
    }
    private void Update()
    {
        if (driveable)
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
        float multipliedDriveForce = input.y * motorForce;
        driveForce = new Vector3(0, 0, multipliedDriveForce) * Time.fixedDeltaTime;
        #region Accel/Decel-Physics
        if (input.y != 0)
        {
            currentVel = Vector3.MoveTowards(currentVel, driveForce, accelerationForce * Time.fixedDeltaTime);
        }
        else if(input.y == 0 && currentVel.z != 0)
        {
            currentVel = Vector3.MoveTowards(currentVel, Vector3.zero, decelerationForce * Time.fixedDeltaTime);
        }
        #endregion
        float multipliedRotationForce = input.x * rotationForce;

        tankScript.Controller.transform.Rotate(0, multipliedRotationForce * Time.fixedDeltaTime, 0);
        direction = tankScript.Controller.transform.TransformDirection(currentVel);
        tankScript.Controller.Move(direction);
        #endregion
 }
    #endregion

    #region EngineRev
    private void EngineRev(Vector2 input)
    {
        #region Engine Rev Audio (Based on Input)
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
        #endregion
    }
    #endregion

    #region Audio Manager

    private void SFXMultiplierSetup()
    {
        //Gets the top speed. motorForceDivdided is the velocity value that prints out whenever you debug currentVel.z
        float motorForceDivided = motorForce / 50;
        //Sets a multiplier value for currentvalue to make sure the final values always becomes the same no matter what the top speed is
        currentVelMultiplier = 0.1f / motorForceDivided;
    }

    private void VolumeManager()
    {
        //Absolutes currentVel
        float tankVelAbs = Mathf.Abs(currentVel.z);
        //Sets a multiplier value for currentvalue to make sure the top value never goes below or higher than 0.1
        float tankVelForAudio = tankVelAbs * currentVelMultiplier;

        engineIdleSource.volume = 1 - tankVelForAudio * 10;

        engineThrottleSource.pitch = 10 * tankVelForAudio;
        engineThrottleSource.volume = 10 * tankVelForAudio - 0.3f;
    }    
    IEnumerator EngineStartUpSound()
    {
        engineStartup.Play(engineStartupSource);
        
        yield return new WaitForSeconds(1.1568f);
        
        engineIdle.Play(engineIdleSource);
        engineThrottle.Play(engineThrottleSource);
        
        driveable = true;
    }
    #endregion
}
