using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private Animator anim;
    public AudioSource open;
    public AudioSource close;
    public float rootRot;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            anim.SetBool("isOpen", true);
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
                open.Play();
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            anim.SetBool("isOpen", false);
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
                close.Play();
        }
    }
}
