using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CommanderNPC : MonoBehaviour
{
    public GameObject firstEnemy;
    public GameObject secEnemy;
    public LayerMask whatIsGround, whatIsEnemy, whatIsGun, whatIsGrenade;
    public Vector3 walkPoint;
    public float walkPointRange;
    public float attackDelay = 1f;
    public float throwDelay = 8f;
    public float sightRange, attackRange;
    public int hp = 100;

    NavMeshAgent nma;
    Animator anim;
    bool walkPointSet;
    bool attacked, threwGrenade;
    bool holdsGun, holdsGrenade;
    bool isAlive;
    bool inSightRange, inAttackRange;
    bool setPoint, gunPoint;
    int childNum;


    // Start is called before the first frame update
    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        holdsGun = false;
        setPoint = false;
        holdsGrenade = false;
        attacked = false;
        threwGrenade = false;
        walkPointSet = false;
        inAttackRange = false;
        inSightRange = false;
        gunPoint = false;
        isAlive = true;
        nma.SetDestination(transform.position);
        anim.SetInteger("mode", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (firstEnemy.transform.GetChild(0).gameObject.activeSelf)
                childNum = 0;
            else
                childNum = 1;
            if (holdsGun)
            {
                inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
                inAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);
                if (inSightRange && !inAttackRange)
                    ChaseEnemy();
                else if (inSightRange && inAttackRange)
                {
                    bool firstEnemyAlive, secEnemyAlive;
                    firstEnemyAlive = firstEnemy.transform.GetChild(childNum).GetComponent<PlayerMove>().GetIsAlive();
                    secEnemyAlive = secEnemy.GetComponent<Follower>().GetIsAlive();
                    if (firstEnemyAlive || secEnemyAlive)
                        AttackEnemy();
                }
            }
            if (holdsGrenade && !threwGrenade)
            {
                inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
                if (inSightRange)
                {
                    threwGrenade = true;
                    GetComponent<ThrowAGrenade>().ThrowGrenade();
                    Invoke(nameof(ResetGrenade), throwDelay);
                }
            }
            if (!holdsGrenade)
            {
                inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsGrenade);
                if (inSightRange)
                    GetGrenade();
            }
            if (!holdsGun)
            {
                inSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsGun);
                if (inSightRange)
                    GetGun();
            }
            if (!inSightRange && !inAttackRange)
                Search();
        }
    }

    private void Search()
    {
        if (!holdsGun && anim.GetInteger("mode") != 1)
            anim.SetInteger("mode", 1);
        else if (holdsGun && anim.GetInteger("mode") != 3)
            anim.SetInteger("mode", 3);
        
        if (!walkPointSet)
            SearchWalkPoint();
        if (!setPoint)
        {
                nma.SetDestination(walkPoint);
                setPoint = true;
        }

        if (nma.remainingDistance < 1f)
        {
            walkPointSet = false;
            setPoint = false;
        }

    }

    private void SearchWalkPoint()
    {
        float randZ = Random.Range(-walkPointRange, walkPointRange);
        float randX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void GetGun()
    {
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

        if (!gunPoint)
            nma.SetDestination(gunPos);

        Vector3 distanceFromGun = transform.position - gunPos;
        if (distanceFromGun.magnitude < 0.5f)
        {
            holdsGun = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            gun.SetActive(false);
            gunPoint = true;
        }
    }

    private void GetGrenade()
    {
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
        if (distanceFromGrenade.magnitude < 0.5f)
        {
            holdsGrenade = true;
            grenade.SetActive(false);
        }
    }

    private void ChaseEnemy()
    {
        if (anim.GetInteger("mode") != 3)
            anim.SetInteger("mode", 3);
        Vector3 distanceFromFirstEnemy = transform.position - firstEnemy.transform.position;
        Vector3 distanceFromSecEnemy = transform.position - secEnemy.transform.position;
        if (distanceFromFirstEnemy.magnitude < distanceFromSecEnemy.magnitude)
        {
            nma.SetDestination(firstEnemy.transform.GetChild(childNum).transform.position);
        }
        else
            nma.SetDestination(secEnemy.transform.position);
    }

    private void AttackEnemy()
    {
        if (anim.GetInteger("mode") != 2)
            anim.SetInteger("mode", 2);
        nma.SetDestination(transform.position);
        Vector3 distanceFromFirstEnemy = transform.position - firstEnemy.transform.GetChild(childNum).transform.position;
        Vector3 distanceFromSecEnemy = transform.position - secEnemy.transform.position;
        if (distanceFromFirstEnemy.magnitude < distanceFromSecEnemy.magnitude)
            transform.LookAt(firstEnemy.transform.GetChild(childNum).transform);
        else
            transform.LookAt(secEnemy.transform);
        if (!attacked)
        {
            // Attack Code (Shoot)
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
            gameObject.GetComponent<BoxCollider>().gameObject.SetActive(false);
            gameObject.layer = 0;
            transform.GetChild(0).gameObject.SetActive(false);
            anim.SetInteger("mode", 4);
        }
    }  
    
    public bool GetHoldsGun()
    {
        return holdsGun;
    }

    public bool GetHoldsGrenade()
    {
        return holdsGrenade;
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }
}




