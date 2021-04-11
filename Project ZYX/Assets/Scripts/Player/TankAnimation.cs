using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class TankAnimation : MonoBehaviour
{
    [SerializeField] private Animator tankAnimator;
    private Tank tank;
    private ParticleSystem bubbleParticle;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        tank.Tick += UpdateBubbles;
    }



    public void IdleAnim()
    {
        tankAnimator.SetTrigger("IdleAnim");
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void MoveForwardAnim()
    {
        tankAnimator.SetTrigger("MoveForward");
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void MoveBackwardAnim()
    {
        tankAnimator.SetTrigger("MoveBackward");
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void StopForwardAnim()
    {

    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void StopBackwardAnim()
    {

    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ShootAnim()
    {
        //tankAnimator.SetTrigger("Shoot");
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



    private void UpdateBubbles(float Velocity)
    {
        ParticleSystem.EmissionModule i = bubbleParticle.emission;
        ParticleSystem.MinMaxCurve tempCurve = i.rateOverTime;
        tempCurve.constant = 9f * Velocity;
        i.rateOverTime = tempCurve;
    }


    private void OnLoadStats(TankRef i)
    {
        bubbleParticle = i.tankBubblesParticles;
    }
}
