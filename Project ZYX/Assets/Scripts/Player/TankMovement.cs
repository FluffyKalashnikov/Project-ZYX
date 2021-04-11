using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TankMovement : MonoBehaviour
{
    [SerializeField] private Tank tankScript;
    [SerializeField] private TankAudio tankAudioScript;
    [SerializeField] private TankAnimation tankAnimationScript;

    // PRIVATE REFERENCES
    InputAction moveAction;
    PlayerInput playerInput;

    #region Stats
    [Header("Force Values")]
    public float motorForce;
    [SerializeField] private float accelerationForce;
    [SerializeField] private float decelerationForce;
    [SerializeField] private float rotationForce;

    [Header("Velocity Max for engine sounds")]
    public float velocityMax;
    #endregion

    private Vector3 driveForce;
    private Vector3 currentVel;
    private Vector3 direction;

    private bool ifReving = false;

    public float Timer;

    private Animator animator = null;

    #region Setup
    private void Awake()
    {
        // 1. GET REFERENCES
        tankScript = GetComponent<Tank>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions.FindAction("Move");
    }
    void Start()
    {
        //StartCoroutine(tankAudioScript.EngineStartUpSound());
    }
    private void Update()
    {
        BaseMovement(moveAction.ReadValue<Vector2>());
        EngineRev(moveAction.ReadValue<Vector2>());
        tankScript.Tick?.Invoke(currentVel.magnitude);
        
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

    private void BaseMovement(Vector2 input)
    {
        #region Actual Movement
        float multipliedMotorForce = input.y * motorForce;
        driveForce = new Vector3(0, 0, multipliedMotorForce);
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
        tankScript.Controller.Move(direction * Time.deltaTime);
        animator?.SetFloat("Speed", currentVel.z);
        #endregion  
    }

    private void EngineRev(Vector2 input)
    {
        if (input.y > 0 && input.y <= 0.3 && !ifReving || input.y < 0 && input.y >= -0.3 && !ifReving)
        {
            tankAudioScript.EngineRevLow();
            ifReving = true;
        }
        else if (input.y == 0 && ifReving)
        {
            ifReving = false;
        }

        if (input.y > 0.3 && input.y <= 0.6 && !ifReving || input.y < -0.3 && input.y >= -0.6 && !ifReving)
        {
            tankAudioScript.EngineRevMid();
            ifReving = true;
        }
        else if (input.y == 0 && ifReving)
        {
            ifReving = false;
        }

        if (input.y > 0.6 && !ifReving || input.y < -0.6 && !ifReving)
        {
            tankAudioScript.EngineRevHigh();
            ifReving = true;
        }
        else if (input.y == 0 && ifReving)
        {
            ifReving = false;
        }
    }

    private void VolumeManager()
    {
        //Absolutes currentVel
        float tankVelAbs = Mathf.Abs(currentVel.z);
        float velocityScale = tankVelAbs / velocityMax;
        tankAudioScript.EngineSounds(velocityScale);
    }
    public void Enable()
    {
        moveAction.Enable();
    }
    public void Disable()
    {
        moveAction.Disable();
    }

    public void OnLoadStats(TankRef i)
    {
        animator = i.GetComponent<Animator>();
        motorForce = tankScript.TankAsset.Speed;
        velocityMax = tankScript.TankAsset.VelocityMax;
    }
}
