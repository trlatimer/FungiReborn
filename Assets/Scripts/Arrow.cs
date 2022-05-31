using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody rig;

    public float lifetime;
    public float damage;
    public float force;

    public AudioSource playerAudio;
    public AudioClip arrowHit;
    public AudioClip arrowMiss;

    private DateTime spawnTimestamp;

    private void Start()
    {
        spawnTimestamp = DateTime.Now;
        rig.AddRelativeForce(Vector3.forward * force);
        playerAudio = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (spawnTimestamp.AddSeconds(lifetime) < DateTime.Now)
            Destroy(gameObject);
        PointForward();
    }

    private void PointForward()
    {
        float combVelocity = Mathf.Sqrt(rig.velocity.x * rig.velocity.x + rig.velocity.z * rig.velocity.z);
        float fallAngle = -1 * Mathf.Atan2(rig.velocity.y, combVelocity) * 180 / Mathf.PI;
        transform.eulerAngles = new Vector3(fallAngle, transform.eulerAngles.y, transform.eulerAngles.x);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") return;
        if (collision.gameObject.tag == "Armor") return;

        bool hitPlayer = false;
        if (collision.gameObject.transform.parent != null)
        {
            IDamageable damageable = collision.gameObject.transform.parent.GetComponentInParent<IDamageable>();
            if (damageable is object)
            {
                damageable.TakeDamage(damage);
                playerAudio.PlayOneShot(arrowHit);
                hitPlayer = true;
            } 
        }
        if (!hitPlayer) playerAudio.PlayOneShot(arrowMiss);
        Destroy(gameObject);
    }
}
