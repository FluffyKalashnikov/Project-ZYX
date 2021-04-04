using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private float damage = 0f;

    private Rigidbody    rb       = null;
    private Tank         owner    = null;
    private bool         hasHit   = false;
    private new Collider collider = null;

    private void Awake() 
    {
        rb       = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }
    public void Init(float velocity, Tank sender)
    {
        owner = sender;

        rb.AddForce(transform.forward * velocity, ForceMode.Impulse);
        foreach(var col in sender.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(collider, col);
        }
    }

    private void Update()
    {
        if (!hasHit && rb.velocity != Vector3.zero) 
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }




    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        IDamageable hit = collision.gameObject.GetComponentInParent<IDamageable>();

        hit?.TakeDamage
        (
            new DamageInfo
            (
                damage,
                DamageType.ShellImpact,
                owner,
                hit
            )
        );

        hasHit = true;
        rb.useGravity = true;
        Tank.IgnoreCollision(collider);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
