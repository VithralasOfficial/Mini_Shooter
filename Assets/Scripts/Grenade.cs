using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 4f;
    public float blastRadius = 5f;
    public int damage = 30;
    public GameObject explosionEffect;
    
    private AudioSource boom;

    float explosionForce = 700f;
    float countdown;
    bool hasExploded = false;

    // Start is called before the first frame update
    void Start()
    {
        boom = GetComponent<AudioSource>();
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!boom.isPlaying)
            if (!hasExploded)
                boom.Play();
            else
                Destroy(gameObject);

        countdown -= Time.deltaTime;
        if (countdown <= 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] hitObjects = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider neabyObject in hitObjects)
        {
            Rigidbody rb = neabyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
                if (rb.gameObject.name.Equals("Remy"))
                    rb.gameObject.GetComponent<CommanderNPC>().TakeDamage(damage);
                else if (rb.gameObject.name.Equals("Joe") || rb.gameObject.name.Equals("kachujin"))
                    rb.gameObject.GetComponent<Follower>().TakeDamage(damage);
                else if (rb.gameObject.name.Equals("Player1"))
                    rb.gameObject.GetComponent<PlayerMove>().TakeDamage(damage);

                else if (rb.gameObject.name.Equals("Player2"))
                    rb.gameObject.GetComponent<PlayerMove>().TakeDamage(damage);
            }
        }
        if (boom.isPlaying)
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            mr.enabled = false;
        }
    }
}
