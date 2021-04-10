using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankTurret : MonoBehaviour
{
    #region Setup Var
    [SerializeField] private Tank tankScript;
    [SerializeField] private TankAudio tankAudioScript;

    // PRIVATE REFERENCES
    InputAction moveAction;
    PlayerInput playerInput;

    private Vector3 LookVector = Vector2.zero;

    private bool spinningBool;

    private float rotationSpeed;
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
    [Header("Turret Objects")]
    [SerializeField] private GameObject turret;
    #endregion

    #region Setup
    private void Awake()
    {
        // 1. GET REFERENCES
        tankScript = GetComponent<Tank>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions.FindAction("Look");
    }
    private void Update()
    {
        ILook();
    }
    #endregion

    #region Spin Functions
    private void TurretRotation(Vector2 input)
    {
        turret.transform.rotation = Quaternion.LookRotation(-LookVector, Vector3.up);


        /*#region Audio
        if (mousepos.x != 0 && !spinningBool)
        {
            tankAudioScript.TurretSoundPlay();
            spinningBool = true;
        }
        else if (mousepos.x == 0 && spinningBool)
        {
            tankAudioScript.TurretSoundStop();
            spinningBool = false;
        }
        #endregion*/
    }
    void ILook()
    {
        TurretRotation(LookVector = VectorCalc());

        Vector3 VectorCalc()
        {

            switch (playerInput.currentControlScheme)
            {
                case "Gamepad":

                    Vector2 input = moveAction.ReadValue<Vector2>();


                    return new Vector3(input.x, 0f, input.y);
                case "Keyboard&Mouse":


                    // VARIABLES
                    Vector3 delta = Vector3.zero;
                    float distance = 0f;
                    Ray ray = Camera.main.ScreenPointToRay(moveAction.ReadValue<Vector2>());
                    Plane plane = new Plane(Vector3.up, 0f);

                    plane.Raycast(ray, out distance);
                    {
                        Vector3 point = ray.GetPoint(distance);

                        delta.x = point.x;
                        delta.z = point.z;
                    }

                    delta -= tankScript.Controller.transform.position;
                    delta.y = 0f;

                    return delta.normalized;
                default: Debug.Log("Input Device not supported!"); return Vector3.zero;
            }

        }

    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    #endregion

    public void OnLoadStats(TankRef i)
    {
        turret = tankScript.TankRef.TurretTransform.gameObject;
    }
}
