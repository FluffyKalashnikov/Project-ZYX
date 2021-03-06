using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class OLD_Shell : MonoBehaviour
{
    [SerializeField] private float          damage         = 1f;
    [SerializeField] private float          velocity       = 20f;
    [SerializeField] private ParticleSystem impactParticle = null;
    //[SerializeFied]
    

    private new Rigidbody   rigidbody = null;
    private new CapsuleCollider collider  = null;
    private     TankOLD         owner     = null;


    public Action<Collision> OnImpact;


    public void Init(TankOLD sender)
    {
        rigidbody = GetComponent<Rigidbody>();
        collider  = GetComponent<CapsuleCollider>();
        owner     = sender;

        //Physics.IgnoreCollision(collider, owner.controller);
        rigidbody.velocity = transform.forward * velocity;
        Destroy(gameObject, 7f);

        OnImpact += Explode;
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnImpact(collision);
    }




    public virtual void Explode(Collision collision)
    {
        if (impactParticle) Destroy(Instantiate(impactParticle, transform.position, transform.rotation), 3f);
        Destroy(gameObject);
    }
}
