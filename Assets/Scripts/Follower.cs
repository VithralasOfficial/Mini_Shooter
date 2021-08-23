using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Follower : MonoBehaviour
{
    public GameObject firstEnemy;
    public GameObject secEnemy;
    public GameObject commander;
    public LayerMask whatIsGround, whatIsEnemy, whatIsGun, whatIsGrenade;
    public float attackDelay = 1f;
    public float grenadeThrowDelay = 8f;
    public float sightRange, attackRange;
    public int hp = 100;

    Animator anim;
    NavMeshAgent nma;
    bool holdsGun, holdsGrenade, isAlive, gettingGun, seeEnemy, threwGrenade;
    bool inSightRange, inAttackRange, attacked, gettingGrenade;
    int childNum;

    // Start is called before the first frame update
    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        holdsGun = false;
        holdsGrenade = false;
        attacked = false;
        threwGrenade = false;
        gettingGun = false;
        inAttackRange = false;
        inSightRange = false;
        seeEnemy = false;
        isAlive = true;
        anim.SetInteger("mode", 0);
        if (gameObject.name.Equals("kachujin"))
            nma.SetDestination(commander.transform.position);
        else
            nma.SetDestination(commander.transform.GetChild(childNum).position);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (gameObject.name.Equals("Joe"))
            {
                if (commander.transform.GetChild(0).gameObject.activeSelf)
                    childNum = 0;
                else
                    childNum = 1;
            }
            else
            {
                if (firstEnemy.transform.GetChild(0).gameObject.activeSelf)
                    childNum = 0;
                else
                    childNum = 1;
            }

            if (!holdsGrenade)
            {
                if (gameObject.name.Equals("kachujin"))
                {
                    CommanderNPC cn = commander.GetComponent<CommanderNPC>();
                    inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsGrenade);
                    if (inSightRange && cn.GetHoldsGrenade())
                        GetGrenade();
                }
                else
                {
                    PlayerMove pm = commander.transform.GetChild(childNum).gameObject.GetComponent<PlayerMove>();
                    inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsGrenade);
                    if (inSightRange && pm.GetHoldsGrenade())
                        GetGrenade();
                }
            }
            if (!holdsGun)
            {
                if (gameObject.name.Equals("kachujin"))
                {
                    CommanderNPC cn = commander.GetComponent<CommanderNPC>();
                    inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsGun);
                    if (inSightRange && cn.GetHoldsGun())
                        GetGun();
                }
                else
                {
                    PlayerMove pm = commander.transform.GetChild(childNum).gameObject.GetComponent<PlayerMove>();
                    inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsGun);
                    if (inSightRange && pm.GetHoldsGun())
                        GetGun();
                }
            }
            if (holdsGun)
            {
                inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
                inAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);
                if (inSightRange)
                {
                    bool firstEnemyAlive, secEnemyAlive;
                    if (firstEnemy.name == "Remy")
                        firstEnemyAlive = firstEnemy.GetComponent<CommanderNPC>().GetIsAlive();
                    else
                        firstEnemyAlive = firstEnemy.transform.GetChild(childNum).gameObject.GetComponent<PlayerMove>().GetIsAlive();
                    secEnemyAlive = secEnemy.GetComponent<Follower>().GetIsAlive();
                    seeEnemy = true;
                    if (inAttackRange)
                    {
                        if (firstEnemyAlive || secEnemyAlive)
                            AttackEnemy();
                        else
                        {
                            if (gameObject.name.Equals("kachujin"))
                                nma.SetDestination(commander.transform.position);
                            else
                                nma.SetDestination(commander.transform.GetChild(childNum).position);
                        }
                    }
                    else
                    {
                        if (firstEnemyAlive || secEnemyAlive)
                            ChaseEnemy();
                        else
                        {
                            if (gameObject.name.Equals("kachujin"))
                                nma.SetDestination(commander.transform.position);
                            else
                                nma.SetDestination(commander.transform.GetChild(childNum).position);
                        }

                    }
                }
                else
                    seeEnemy = false;
            }
            if (holdsGrenade && !threwGrenade)
            {
                inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
                if (inSightRange)
                {
                    threwGrenade = true;
                    GetComponent<ThrowAGrenade>().ThrowGrenade();
                    Invoke(nameof(ResetGrenade), grenadeThrowDelay);
                }    
            }
            if (!gettingGun && !gettingGrenade && !seeEnemy)
            {
                if (Vector3.Distance(transform.position, commander.transform.position) > 1f)
                {
                    nma.isStopped = true;
                    if (gameObject.name.Equals("kachujin"))
                        nma.SetDestination(commander.transform.position);
                    else
                        nma.SetDestination(commander.transform.GetChild(childNum).position);
                }
            }
            if (nma.remainingDistance > 1.5f)
            {
                nma.isStopped = false;
                if (!holdsGun)
                {
                    if (anim.GetInteger("mode") != 1)
                        anim.SetInteger("mode", 1);
                }
                else
                    if (anim.GetInteger("mode") != 3)
                    anim.SetInteger("mode", 3);
            }
            else
            {
                if (!holdsGun)
                    anim.SetInteger("mode", 0);
                else
                    anim.SetInteger("mode", 2);
            }
        }
    }

    private void GetGun()
    {
        gettingGun = true;
        if (anim.GetInteger("mode") != 1)
            anim.SetInteger("mode", 1);
        Collider[] objects = Physics.OverlapSphere(transform.position, sightRange);
        GameObject gun = gameObject; // Just to avoid the warning. 
        Vector3 gunPos = transform.position; // Just to avoid the warning. The function executes only when there is a gun in range.
        foreach (Collider obj in objects)
            if (obj.CompareTag("Gun"))
            {
                gunPos = obj.transform.position;
                gun = obj.transform.gameObject;
                break;
            }

        nma.SetDestination(gunPos);

        Vector3 distanceFromGun = transform.position - gunPos;
        if (distanceFromGun.magnitude < 0.3f)
        {
            holdsGun = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            gun.SetActive(false);
            gettingGun = false;
        }
    }

    private void GetGrenade()
    {
        gettingGrenade = true;
        if (anim.GetInteger("mode") != 1)
            anim.SetInteger("mode", 1);
        Collider[] objects = Physics.OverlapSphere(transform.position, sightRange);
        GameObject grenade = gameObject; // Just to avoid the warning. 
        Vector3 grenadePos = transform.position; // Just to avoid the warning. The function executes only when there is a gun in range.
        foreach (Collider obj in objects)
            if (obj.CompareTag("Grenade"))
            {
                grenadePos = obj.transform.position;
                grenade = obj.transform.gameObject;
                break;
            }

        nma.SetDestination(grenadePos);

        Vector3 distanceFromGrenade = transform.position - grenadePos;
        if (distanceFromGrenade.magnitude < 0.3f)
        {
            holdsGrenade = true;
            grenade.SetActive(false);
            gettingGrenade = false;
        }
    }

    private void ChaseEnemy()
    {
        Vector3 distanceFromFirstEnemy;
        if (anim.GetInteger("mode") != 3)
            anim.SetInteger("mode", 3);
        if (gameObject.name.Equals("kachujin"))
            distanceFromFirstEnemy = transform.position - firstEnemy.transform.GetChild(childNum).position;
        else
            distanceFromFirstEnemy = transform.position - firstEnemy.transform.position;
        Vector3 distanceFromSecEnemy = transform.position - secEnemy.transform.position;
        if (distanceFromFirstEnemy.magnitude < distanceFromSecEnemy.magnitude)
        {
            if (gameObject.name.Equals("kachujin"))
                nma.SetDestination(firstEnemy.transform.GetChild(childNum).position);
            else
                nma.SetDestination(firstEnemy.transform.position);
        }
        else
            nma.SetDestination(secEnemy.transform.position);
    }

    private void AttackEnemy()
    {
        Vector3 distanceFromFirstEnemy;
        bool bothAlive = true;
        bool firstAlive, secAlive;
        if (anim.GetInteger("mode") != 2)
            anim.SetInteger("mode", 2);
        nma.SetDestination(transform.position);
        if (gameObject.name.Equals("kachujin"))
            distanceFromFirstEnemy = transform.position - firstEnemy.transform.GetChild(childNum).position;
        else
            distanceFromFirstEnemy = transform.position - firstEnemy.transform.position;
        Vector3 distanceFromSecEnemy = transform.position - secEnemy.transform.position;
        if (gameObject.name.Equals("kachujin"))
        {
            firstAlive = firstEnemy.transform.GetChild(childNum).gameObject.GetComponent<PlayerMove>().GetIsAlive();
            secAlive = secEnemy.GetComponent<Follower>().GetIsAlive();
            if (firstAlive && !secAlive)
            {
                bothAlive = false;
                if (gameObject.name.Equals("kachujin"))
                    transform.LookAt(firstEnemy.transform.GetChild(childNum));
                else
                    transform.LookAt(firstEnemy.transform);
            }
            else if (!firstAlive && secAlive)
            {
                bothAlive = false;
                transform.LookAt(secEnemy.transform);
            }
            else if (!firstAlive && !secAlive)
                bothAlive = false;
            else
                bothAlive = true;
        }
        else
        {
            firstAlive = firstEnemy.GetComponent<CommanderNPC>().GetIsAlive();
            secAlive = secEnemy.GetComponent<Follower>().GetIsAlive();
            if (firstAlive && !secAlive)
            {
                bothAlive = false;
                transform.LookAt(firstEnemy.transform);
            }
            else if (!firstAlive && secAlive)
            {
                bothAlive = false;
                transform.LookAt(firstEnemy.transform);
            }
            else if (!firstAlive && !secAlive)
                bothAlive = false;
            else
                bothAlive = true;
        }
        if (bothAlive)
        {
            if (distanceFromFirstEnemy.magnitude < distanceFromSecEnemy.magnitude)
            {
                if (gameObject.name.Equals("kachujin"))
                    transform.LookAt(firstEnemy.transform.GetChild(childNum));
                else
                    transform.LookAt(firstEnemy.transform);
            }
            else
                transform.LookAt(secEnemy.transform);
        }
        if (!attacked)
        {
            SimpleShoot sp = gameObject.transform.GetChild(0).GetChild(0).GetComponent<SimpleShoot>();
            sp.Fire();

            attacked = true;
            Invoke(nameof(ResetAttack), attackDelay);
        }
    }

    private void ResetAttack()
    {
        attacked = false;
    }

    private void ResetGrenade()
    {
        threwGrenade = false;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            isAlive = false;
            nma.isStopped = true;
            gameObject.layer = 0;
            transform.GetChild(0).gameObject.SetActive(false);
            anim.SetInteger("mode", 4);
        }
    }  
    
    public bool GetIsAlive()
    {
        return isAlive;
    }
}
