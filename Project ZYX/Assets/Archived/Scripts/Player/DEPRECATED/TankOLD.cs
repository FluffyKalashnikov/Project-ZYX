using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class TankOLD : MonoBehaviour
{
    [Header("Tank Stats")]
    [SerializeField] private float      maxHealth   = 5f;
    [SerializeField] private float      health      = 1f;
    [SerializeField] private GameObject shellPrefab = null;

    [Space(15)]
    [SerializeField] private float maxSpeed          = 7f;
    [SerializeField] private float acceleration      = 11f;
    [SerializeField] private float deceleration      = 5f;
    [SerializeField] private float boostDeceleration = 15f;

    [Header("References")]
    [SerializeField] private Transform           tankModel   = null;
    [SerializeField] private Transform           tankTurret  = null;

    // VARIABLES
    private Vector3 LookVector   = Vector2.zero;
    private Vector3 LookRotation = Vector3.zero;


    // ACTIONS
    private ActionAsset IAsset = null;
    private Action      OnTick;
    private Action      OnFire;


  
    void Update() => OnTick?.Invoke();
    void Awake()
    {
        // ACTIONS
        IAsset = new ActionAsset();

        OnFire += IFire;
        OnTick += IMove;
        OnTick += ILook;

        IAsset.Player.Fire.performed += ctx => OnFire();

        void IMove()
        {
            if (IAsset.Player.Move.enabled)
                Move(IAsset.Player.Move.ReadValue<Vector2>());
        }
        void ILook()
        {
            if (IAsset.Player.Look.enabled == false) return;

            //Look(LookVector = VectorCalc());


            /*
            Vector3 VectorCalc()
            {
                
                switch (playerInput.currentControlScheme)
                {
                    case "Gamepad": 
                        
                        Vector2 input = IAsset.Player.Look.ReadValue<Vector2>();


                        return new Vector3(input.x, 0f, input.y);
                    case "Keyboard&Mouse":


                        // VARIABLES
                        Vector3 delta      =  Vector3.zero;
                        float   distance   =  0f;
                        Ray     ray        =  camera.ScreenPointToRay(IAsset.Player.Look.ReadValue<Vector2>());
                        Plane   plane      =  new Plane(Vector3.up, 0f);

                        plane.Raycast(ray, out distance);
                        {
                            Vector3 point = ray.GetPoint(distance);

                            delta.x = point.x;
                            delta.z = point.z;
                        }

                        delta -= controller.transform.position;
                        delta.y = 0f;
                        
                        return delta.normalized;
                    default: Debug.Log("Input Device not supported!"); return Vector3.zero;
                }
                
            }
            */
        }
        void IFire()
        {
            if (IAsset.Player.Move.enabled)
                Fire();
        }
    }

    

    public virtual void Move(Vector2 input)
    {
        Vector3 vector = new Vector3
        (
            input.x * maxSpeed * Time.deltaTime, 
            0f, 
            input.y * maxSpeed * Time.deltaTime
        );


        //controller.Move(vector);

        // ROTATION EFFECT
        if (input != Vector2.zero)
        {
            //tankModel.rotation = Quaternion.LookRotation(controller.velocity.normalized);
        }
    }
    public virtual void Look(Vector3 input)
    {
        tankTurret.rotation = Quaternion.LookRotation(LookVector, Vector3.up);
    }
    public virtual void Fire()
    {
        Debug.Log("Fire!");

        GameObject shell = Instantiate(shellPrefab, tankTurret.position + tankTurret.forward, tankTurret.rotation);
        shell.GetComponent<OLD_Shell>().Init(this);
    }









    private void OnEnable()  => IAsset.Player.Enable();
    private void OnDisable() => IAsset.Player.Disable();
}
