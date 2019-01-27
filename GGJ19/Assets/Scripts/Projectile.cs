using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    ParticleSystem hitSparksPrefab;
    private const float minVelocity = 10.0f;
    Rigidbody rb;
    private float constantSpeed = 0;
    [HideInInspector] const float explosionForce = 100;
    [HideInInspector] const float explosionRadius = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hitSparksPrefab = Resources.Load<ParticleSystem>("Particle Effects/VfxHitSparks");
    }

    // Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (constantSpeed == 0)
        {
            constantSpeed = rb.velocity.magnitude;
        }
        else
        {
            rb.velocity = rb.velocity.normalized * constantSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Vector3 dir = (transform.position - collision.transform.position).normalized;
            Vector3 vel = rb.velocity;

            ParticleSystem hitSparks = Instantiate(hitSparksPrefab, transform.position, transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0)));

            Destroy(hitSparks, hitSparks.startLifetime);

        }

        foreach (var asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            asteroid.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        Destroy(gameObject, 0);
    }

}
