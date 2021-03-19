using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankShoot : MonoBehaviour
{
    #region Setup Var
    ActionAsset actionAsset;
    [SerializeField] private Tank tankScript;
    [SerializeField] private TankAudio tankAudioScript;
    private bool gunActive;
    private bool launchActive;
    private float chargeStrength;
    #endregion

    [SerializeField] private Transform bulletPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Shell bulletScript;

    public float timer;
    public float timer2;

    #region Setup
    private void Awake()
    {
        actionAsset = new ActionAsset();
        tankScript = GetComponent<Tank>();
    }
    void Start()
    {
        gunActive = false;
        launchActive = false;
        actionAsset.Player.Enable();
        actionAsset.Player.Fire.performed += ctx => gunActive = true;
        actionAsset.Player.Fire.performed += ctx => launchActive = false;
        actionAsset.Player.Fire.canceled += ctx => gunActive = false;
        actionAsset.Player.Fire.canceled += ctx => launchActive = true;
    }
    void Update()
    {
        if(gunActive == true && launchActive == false)
        {
            ChargeFunction();
        }
        else if(gunActive == false && launchActive == true)
        {
            LaunchFunction();
        }
    }
    #endregion
    private void ChargeFunction()
    {
        if (timer2 < 0)
        {
            timer2 -= Time.deltaTime;
            if (chargeStrength <= 30)
            {
                chargeStrength = chargeStrength + 10 * (Time.deltaTime * 6);
            }
        }
        else if (timer2 > 0)
        {
            timer2 -= Time.deltaTime;
            if (chargeStrength <= 30)
            {
                chargeStrength = chargeStrength + 10 * Time.deltaTime;
            }
        }
       

    }
    private void LaunchFunction()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            for (int i = 0; i <= 2; i++)
            {
                tankAudioScript.CannonFire();
                var projectile = Instantiate(bullet, bulletPoint.position, bulletPoint.rotation);
                projectile.GetComponentInChildren<Shell>().Init(chargeStrength, tankScript);
               
            }
            chargeStrength = 0;
            Tank.OnTankFire?.Invoke(tankScript); // Fires of OnTankFire event
            launchActive = false;
        }
        else if (timer < 0)
        {
            timer -= Time.deltaTime;
            tankAudioScript.CannonFire();
            var projectile = Instantiate(bullet, bulletPoint.position, bulletPoint.rotation);
            projectile.GetComponentInChildren<Shell>().Init(chargeStrength, tankScript);
            chargeStrength = 0;
            Tank.OnTankFire?.Invoke(tankScript); // Fires of OnTankFire event
            launchActive = false;
        }
       
    }
}
