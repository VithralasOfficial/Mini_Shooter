using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float speed = 12f;
    public float gravity = -9.8f;
    public float attackDelay;
    public float pickUpRange = 50f; 
    public int hp = 100;
    private Camera mainCam;
    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
   // private AudioSource footSteps;
    private Vector3 lastPos;
    private Animator anim;
    private bool isAlive;
    private bool attacked;
    private bool holdsGrenade;
    private bool holdsGun;


    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        holdsGrenade = false;
        if (gameObject.name.Equals("Player1"))
            holdsGun = true;
        else
            holdsGun = false;
        attacked = false;
        mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        controller = transform.GetComponent<CharacterController>();
        //footSteps = transform.GetComponent<AudioSource>();
        lastPos = transform.position;
        anim = gameObject.GetComponent<Animator>();
        anim.SetInteger("mode", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            mainCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (transform.position.x != lastPos.x || transform.position.z != lastPos.z)
            {
                if (!holdsGun && anim.GetInteger("mode") != 1)
                    anim.SetInteger("mode", 1);
                else if (holdsGun && anim.GetInteger("mode") != 3)
                    anim.SetInteger("mode", 3);
                lastPos = transform.position;
                //if (!footSteps.isPlaying)
                //    footSteps.Play();
            }
            else
            {
                if (!holdsGun)
                    anim.SetInteger("mode", 0);
                else
                    anim.SetInteger("mode", 2);
            }
           
            if (Input.GetMouseButtonDown(0))
            {
                if (!holdsGun || !holdsGrenade)
                {
                    RaycastHit hit;
                    Transform crosshair = gameObject.transform.GetChild(0).GetChild(0).GetChild(0)
                        .gameObject.GetComponent<SimpleShoot>().GetBarrelLocation();
                    if (Physics.Raycast(crosshair.position, crosshair.forward, out hit, pickUpRange))
                    {
                        if (!holdsGun)
                        {
                            if (hit.transform.CompareTag("Gun"))
                            {
                                hit.transform.gameObject.SetActive(false);
                                holdsGun = true;
                                gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                            }
                        }
                        if (!holdsGrenade)
                        {
                            if (hit.transform.CompareTag("Grenade"))
                            {
                                hit.transform.gameObject.SetActive(false);
                                holdsGrenade = true;
                            }    
                        }
                    }
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (!attacked)
                {
                    attacked = true;
                    SimpleShoot sp = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SimpleShoot>();
                    sp.Fire();
                    Invoke(nameof(ResetAttack), attackDelay);
                }    
            }

        }      
    }

    private void ResetAttack()
    {
        attacked = false;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            gameObject.layer = 0;
            anim.SetInteger("mode", 4);
            isAlive = false;
        }
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public bool GetHoldsGun()
    {
        return holdsGun;
    }

    public bool GetHoldsGrenade()
    {
        return holdsGrenade;
    }  
    
    public int GetHp()
    {
        return hp;
    }
    
    public void SetHoldsGun(bool value)
    {
        holdsGun = value;
    }

    public void SetHoldsGrenade(bool value)
    {
        holdsGrenade = value;
    }

    public void SetHp(int value)
    {
        hp = value;
    }

    public void SetIsAlive(bool value)
    {
        isAlive = value;
    }

}
