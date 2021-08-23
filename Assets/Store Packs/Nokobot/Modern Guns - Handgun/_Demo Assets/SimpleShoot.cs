using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    public int whatIsEnemy;
    public float range = 50f;
    public int damage = 10;
    AudioSource pew;

    void Start()
    {
        pew = gameObject.GetComponent<AudioSource>();
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

    }

    void Update()
    {

    }


    public void Fire()
    {
        //Calls animation on the gun that has the relevant animation events that will fire
        gunAnimator.SetTrigger("Fire");
        pew.Play();
        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, barrelLocation.forward , out hit, range))
        {
            if (hit.transform.gameObject.layer == whatIsEnemy)
            {
                if (hit.transform.name == "Remy")
                {
                    CommanderNPC cp = hit.transform.gameObject.GetComponent<CommanderNPC>();
                    cp.TakeDamage(damage);
                }
                else if (hit.transform.name == "Joe" || hit.transform.name == "kachujin")
                {
                    Follower fr = hit.transform.gameObject.GetComponent<Follower>();
                    fr.TakeDamage(damage);
                }
                else if (hit.transform.name == "Player1" || hit.transform.name.Equals("Player2"))
                {
                    PlayerMove pm = hit.transform.gameObject.GetComponent<PlayerMove>();
                    pm.TakeDamage(damage);
                }
            }
        }
    }
    
    //This function creates the bullet behavior
    void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }

        // Create a bullet and add force on it in direction of the barrel
        GameObject bullet;
        bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        Destroy(bullet, destroyTimer);
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

    public Transform GetBarrelLocation()
    {
        return barrelLocation;
    }
}
