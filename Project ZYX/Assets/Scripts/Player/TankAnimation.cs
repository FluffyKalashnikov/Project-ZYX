using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class TankAnimation : MonoBehaviour
{
    private Animator tankAnimator;
    private Tank tank;
    private ParticleSystem bubbleParticle;

    private int MoveHash;


    private void Awake()
    {
        tank = GetComponent<Tank>();
        MoveHash = Animator.StringToHash("Speed");
        tank.Tick += () => UpdateBubbles(tankAnimator.GetFloat(MoveHash));
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



    private void UpdateBubbles(float Speed)
    {
        SetBubbles(9f * Speed);
    }
    private void SetBubbles(float Emission)
    {
        ParticleSystem.EmissionModule i = bubbleParticle.emission;
        ParticleSystem.MinMaxCurve tempCurve = i.rateOverTime;
        tempCurve.constant = Emission;
        i.rateOverTime = tempCurve;
    }

    private void OnLoadStats(TankRef i)
    {
        tankAnimator   = i.Animator;
        bubbleParticle = i.tankBubblesParticles;
    }
}
