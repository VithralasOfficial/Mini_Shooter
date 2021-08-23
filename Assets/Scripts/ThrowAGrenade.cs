using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAGrenade : MonoBehaviour
{
    public float throwForce = 40f;
    public float throwDelay = 8f;
    public GameObject grenadePrefab;

    bool grenadeThrow;

    private void Start()
    {
        grenadeThrow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name.Equals("Player1") || gameObject.name.Equals("Player2"))
        {
            if (GetComponent<PlayerMove>().GetHoldsGrenade())
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    if (!grenadeThrow)
                    {
                        ThrowGrenade();
                        grenadeThrow = true;
                        Invoke(nameof(resetGrenade), throwDelay);
                    }
                }
            }
        }
    }

    public void ThrowGrenade()
    {
        Vector3 direction = gameObject.transform.forward;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z) + transform.forward * 0.5f; 
        GameObject grenade = Instantiate(grenadePrefab, pos, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (gameObject.name.Equals("Player"))
             direction = gameObject.transform.Find("Main Camera").forward;
        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
    } 
    
    void resetGrenade()
    {
        grenadeThrow = false;
    }
}
