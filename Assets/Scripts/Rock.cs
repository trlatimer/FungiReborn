using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float lifetime;
    public float damage;
    private float timeAlive = 0;

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > lifetime)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") { }
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);

        Destroy(gameObject);
    }
}
