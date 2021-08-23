using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehaviour : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent nma;
    private int index = 0;
    private int lastIndex = 0;
    private float time = 0.0f;
    public float waitingTime = 2.0f;
    public float sittingTime = 5.0f;
    public float delayTime; 
    private bool isWalking;
    public GameObject[] wayPoints;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        anim.SetInteger("mode", 0);
        nma.SetDestination(wayPoints[index].transform.position);
        isWalking = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (nma.remainingDistance > 0.3f)
        {
            if (lastIndex != -1)
                lastIndex = -1;
            if (anim.GetInteger("mode") == 0)
            {
                anim.SetInteger("mode", 1);
            }
        }
        else
        {
            if (anim.GetInteger("mode") == 1)
                anim.SetInteger("mode", 0);
            isWalking = false;
            time += Time.deltaTime;
            if (time % 60 > waitingTime)
            {
                if (anim.GetInteger("mode") != 2 && anim.GetInteger("mode") != 3)
                    isWalking = true;
                if (wayPoints[index].name == "Interactible" && anim.GetInteger("mode") == 0)
                {
                    isWalking = false;
                    transform.Rotate(new Vector3(0, 215, 0));
                    anim.SetInteger("mode", 2);
                    time = 0.0f;
                }
            }

            if (time % 60 > sittingTime && anim.GetInteger("mode") == 2)
            {
                anim.SetInteger("mode", 3);
                time = 0f;
            }
            if (anim.GetInteger("mode") == 3 && time % 60 > delayTime)
            {
                isWalking = true;
            }
            if (isWalking)
            {
                if (index != lastIndex)
                {
                    index = (index + 1) % wayPoints.Length;
                    lastIndex = index;
                }
                nma.SetDestination(wayPoints[index].transform.position);
                anim.SetInteger("mode", 1);
                time = 0f;
            }
        }
    }
}
