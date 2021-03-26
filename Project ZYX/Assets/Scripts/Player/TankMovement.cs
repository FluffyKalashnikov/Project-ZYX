using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TankMovement : MonoBehaviour
{
    //ActionAsset actionAsset;
    InputAction moveAction;
    PlayerInput playerInput;
    [SerializeField] private Tank tankScript;
    [SerializeField] private TankAudio tankAudioScript;
    [SerializeField] private TankAnimation tankAnimationScript;

    public bool moveable = false;

    #region Stats
    [Header("Force Values")]
    [SerializeField] private float motorForce;
    [SerializeField] private float accelerationForce;
    [SerializeField] private float decelerationForce;
    [SerializeField] private float rotationForce;

    [Header("Porpeller Values")]
    [SerializeField] private GameObject propellerBlades;
    [SerializeField] private float propellerIdleSpeed;
    [SerializeField] private float propellerForceMultiplier;

    [Header("Velocity Max for engine sounds")]
    [SerializeField] float velocityMax;
    #endregion

    private Vector3 driveForce;
    private Vector3 currentVel;
    private Vector3 direction;
    private Vector3 propellerVector;

    private bool ifReving = false;

    public float Timer;

    #region Setup
    private void Awake()
    {
        //actionAsset = new ActionAsset();
        moveAction = playerInput.actions.FindAction("Move");
        tankScript = GetComponent<Tank>();
    }
    void Start()
    {
        StartCoroutine(tankAudioScript.EngineStartUpSound());
        //actionAsset.Player.Enable();
        moveAction.Enable();
    }
    private void Update()
    {
        if (moveable)
        {
            //BaseMovement(actionAsset.Player.Move.ReadValue<Vector2>());
            BaseMovement(moveAction.ReadValue<Vector2>());

            //EngineRev(actionAsset.Player.Move.ReadValue<Vector2>());
            //MovementAnimations(actionAsset.Player.Move.ReadValue<Vector2>());
        }
        VolumeManager();
        PropellerSpin();

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
        driveForce = new Vector3(0, 0, input.y * motorForce) * Time.deltaTime;
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
        tankScript.Controller.transform.Rotate(0, input.x * rotationForce * Time.deltaTime, 0);
        direction = tankScript.Controller.transform.TransformDirection(currentVel);
        tankScript.Controller.Move(direction);
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

    /*private void MovementAnimations(Vector2 input)
    {
        if(input.y > 1)
        {
            tankAnimationScript.MoveForwardAnim();
        }
        else if(input.y < 1)
        {
            tankAnimationScript.MoveBackwardAnim();
        }
        else
        {
            tankAnimationScript.IdleAnim();
        }
    }*/

    private void VolumeManager()
    {
        //Absolutes currentVel
        float tankVelAbs = Mathf.Abs(currentVel.z);
        float velocityScale = tankVelAbs / velocityMax;

        tankAudioScript.EngineSounds(velocityScale);
    }

    private void PropellerSpin()
    {
        Vector3 multipliedPropellerValue;
        propellerVector = new Vector3(0, 0, propellerIdleSpeed) * Time.deltaTime;
        multipliedPropellerValue = (currentVel * propellerForceMultiplier) + propellerVector;
        propellerBlades.transform.Rotate(multipliedPropellerValue);
    }

    public void EnableTankMovement()
    {
        moveable = true;
    }
}
