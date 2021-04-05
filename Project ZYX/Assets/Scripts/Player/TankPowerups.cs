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

    [Header("Seamine")]
    [SerializeField] private List<GameObject> seamineList = new List<GameObject>();
    [SerializeField] private Transform tankTransform;
    [SerializeField] private GameObject seamineObject;
    private bool seamineActive;

    [Header("PowerUp Ammounts")]
    [SerializeField] private float HP_Ammount;
    [SerializeField] private int Seamine_Ammount;
    [SerializeField] private float Multishot_Ammount;

    [Header("PowerUp durations")]
    [SerializeField] private float EMP_Duration;
    [SerializeField] private float QuickCharge_Duration;
    [SerializeField] private float Multishot_Duration;
    [SerializeField] private float SpeedBoost_Duration;
    [SerializeField] private float Invincibility_Duration;

    [Header("PowerUp Multipliers")]
    [SerializeField] private float QuickCharge_Multiplier;
    [SerializeField] private float SpeedBoost_Multiplier;
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

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "PU-HP":
                HPMethod();
                break;
            case "PU-Seamine":
                SeamineMethod();
                break;
            case "PU-EMP":
                EMPMethod();
                break;
            case "PU-QuickChagre":
                QuickChargeMethod();
                break;
            case "PU-Multishot":
                MultishotMethod();
                break;
            case "PU-SpeedBost":
                SpeedBoostMethod();
                break;
            case "PU-Invincibility":
                InvincibilityMethod();
                break;
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    #region Pickup Adder
    private void HPMethod()
    {
        tankScript.Health += HP_Ammount;
    }
    private void SeamineMethod()
    {
        for (int i = 0; i < Seamine_Ammount; ++i)
        {
            seamineList.Add(seamineObject);
        }
        seamineActive = true;
    }
    private void EMPMethod()
    {
        Debug.Log("emp");
    }
    private void QuickChargeMethod()
    {
        Debug.Log("quickcharge");
    }
    private void MultishotMethod()
    {
        Debug.Log("multishot");
    }
    private void SpeedBoostMethod()
    {
        Debug.Log("speedbost");
    }
    private void InvincibilityMethod()
    {
        Debug.Log("invincible");
    }
    #endregion

    private void SeamineOperator()
    {
        if (seamineList.Count >= 1 && seamineActive == true)
        {
            Instantiate(seamineObject, new Vector3(tankTransform.position.x, tankTransform.position.y, tankTransform.position.z + 1.7f), Quaternion.identity);
        }
    }
}
