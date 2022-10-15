using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class batController : MonoBehaviour, IDamageable
{

    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator animator;

    [Header("----- Enemy Stats -----")]
    [Range(1, 100)] [SerializeField] int HP;
    [SerializeField] public float chaseSpeed;
    [Range(1, 50)] [SerializeField] int roamRadius;
    [Range(1, 180)] [SerializeField] int fovAngle;
    [Range(1, 10)] [SerializeField] int playerFaceSpeed;
    [SerializeField] float followDistance;
    [SerializeField] float rangedStoppingDistance;
    [SerializeField] float meleeStoppingDistance;
    public bool targetHouse;
    Transform houseTarget;
    public bool targetPlayer;
    public bool houseInRange;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    bool isShooting;

    bool flyingNearby;
    [Range(0, 10)] [SerializeField] float minAttackWaitTime, maxAttackWaitTime;
    bool attackTimerRunning;

    public int meleeDamage;
    public bool isInMeleeRange;
    public List<GameObject> meleeWeapons;
    [SerializeField] bool isMeleeAttacker;
    public bool dealsPosionDamage;
    public int poisonDamage;
    public float timeBetweenPoisonTicks;
    public float poisonTime;
    [SerializeField] bool hasMultipleAttacks;
    [SerializeField] int numberOfAttacks;
    bool isAttacking;

    [SerializeField] List<boostPickUp> boost = new List<boostPickUp>();
    [SerializeField] List<keyPickUp> key = new List<keyPickUp>();
    [SerializeField] private Vector3 lootLocation;


    Vector3 playerDir;
    Vector3 startingPosition;
    bool playerInRange;
    bool playerSeen;
    Vector3 lastPlayerPos;
    //bool searchingForPlayer;
    float speedOrig;
    int randomNumber;

    bool takingDamage;

    [HideInInspector] public bool stunStatusEffectActive;
    [HideInInspector] public float stunTime;
    [HideInInspector] public bool slowStatusEffectActive;
    [Range(0, 1)] public float slowModifier;
    public float slowTime;

    bool isTakingPoisonDamage;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = followDistance;
        if (gameManager.instance.houseTarget.GetComponent<baseController>().targetPositions.Count != 0)
        {
            int rand = Random.Range(0, gameManager.instance.houseTarget.GetComponent<baseController>().targetPositions.Count - 1);
            houseTarget = gameManager.instance.houseTarget.GetComponent<baseController>().targetPositions[rand].transform;
        }
        lastPlayerPos = transform.position;
        agent.speed = chaseSpeed;
        if (isMeleeAttacker)
        {
            agent.stoppingDistance = meleeStoppingDistance;
        }
        else
        {
            agent.stoppingDistance = rangedStoppingDistance;
        }
        startingPosition = transform.position;

        if (transform.localScale.y > 1)
        {
            animator.speed = Mathf.Sqrt(transform.localScale.y / 1) / transform.localScale.y;
        }
        else if (transform.localScale.y < 1)
        {
            animator.speed = Mathf.Sqrt(transform.localScale.y) / transform.localScale.y;
        }

        stunTime = 0;
        slowTime = 0;
    }

    // Update is called once per frame
    void Update()
    {

        // Only run through update if the enemy is enabled ie alive
        if (agent.enabled == true)
        {
            // find direction to player
            playerDir = gameManager.instance.player.transform.position - transform.position;

            animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));

            // Ensures that when the player is dead and respawns, the enemy can retarget
            if (gameManager.instance.playerDeadMenu.activeSelf)
            {
                playerSeen = false;
                playerInRange = false;
                agent.stoppingDistance = 0;
            }

            // checks for knockback animation
            if (!takingDamage)
            {
                if (targetPlayer)
                {
                    if (slowStatusEffectActive)
                    {
                        StartCoroutine(slowDown());
                    }
                    if (!stunStatusEffectActive)
                    {
                        if (!flyingNearby)
                        {
                            moveToPlayer();
                            facePlayer();
                        }
                        else
                        {
                            if (agent.remainingDistance < 0.1f)
                            {
                                createNewPath();
                            }

                            if (!attackTimerRunning)
                            {
                                StartCoroutine(randomAttackTimer());
                            }
                        }
                    }
                    else
                    {
                        StartCoroutine(stunTimer());
                    }
                }
            }
        }
    }


    void moveToPlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
    }

    //get enemy to face the player
    void facePlayer()
    {
        playerDir.y = 0; // we do this because we dont want the enemy looking at player's y position.
        Quaternion rotation = Quaternion.LookRotation(playerDir); //location we want the enemy to look toward
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);  //Lerp is something that happens overtime, instead of snapping to the location. smoothly changes location.
    }




    public void takeDamage(int dmg)
    {
        // check to make sure enemy is not already dead
        if (agent.enabled)
        {
            HP -= dmg;
            animator.SetTrigger("Damage");
            StartCoroutine(waitForDamageAnimToFinish());

            StartCoroutine(flashColor());

            if (HP <= 0)
            {
                enemyDead();
            }
        }
    }

    public void takePoisonDamage(int damage, float poisonTime, float timeBetweenTicks)
    {
        if (!isTakingPoisonDamage)
        {
            StartCoroutine(applyPoison(damage, poisonTime, timeBetweenTicks));
        }
    }

    // poison will flash red but does not cause damage animation or stop speed
    IEnumerator applyPoison(int damage, float poisonTime, float timeBetweenTicks)
    {
        isTakingPoisonDamage = true;
        for (float i = poisonTime; i > 0;)
        {
            if (agent.enabled)
            {
                HP -= damage;

                StartCoroutine(flashColor());

                if (HP <= 0)
                {
                    enemyDead();
                    break;
                }
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(timeBetweenTicks);
            i -= timeBetweenTicks;
        }
        isTakingPoisonDamage = false;
    }

    void enemyDead()
    {
        gameManager.instance.enemyDecrement();
        animator.SetBool("Dead", true);

        if (key.Count > 0)
        {
            Vector3 hoverPos = transform.position;
            hoverPos.y += 1.2f;
            Instantiate(key[0], (hoverPos + lootLocation), transform.rotation);
        }
        else if (boost.Count > 0)
        {
            randomNumber = Random.Range(1, 20);
            if (randomNumber >= 1 && randomNumber <= 5)
            {
                Instantiate(boost[Random.Range(0, boost.Count - 1)], transform.position, transform.rotation);
            }
        }


        agent.enabled = false;


        //// after death, delete colliders... currently worked so takeDamage just does nothing so that enemy bodies can still be interacted with
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }

    }

    IEnumerator flashColor() //changes the color of the enemy.
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }

    IEnumerator waitForDamageAnimToFinish()
    {
        takingDamage = true;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        yield return new WaitForSeconds(1);
        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            takingDamage = false;
        }
    }

    public IEnumerator meleeAttack()
    {

        isAttacking = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(.5f);
        if (!animator.GetBool("Dead"))
        {
            GetComponentInChildren<batMeleeWeaponController>().GetComponent<Collider>().enabled = true;
        }

        yield return new WaitForSeconds(.5f);

        if (!animator.GetBool("Dead"))
        {
            isAttacking = false;
            flyingNearby = true;
            createNewPath();
        }

    }

    void createNewPath()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += gameObject.transform.position;

        NavMeshHit hit;
        // Checks to see if path is valid, sets hit if true
        NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);
    }

    IEnumerator randomAttackTimer()
    {
        attackTimerRunning = true;
        yield return new WaitForSeconds(Random.Range(minAttackWaitTime, maxAttackWaitTime));
        flyingNearby = false;
        attackTimerRunning = false;
    }

    // when player enters follow distance, set playerInRange
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

        }
    }
    // When player leaves follow distance , check if player is seen while exiting
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    IEnumerator stunTimer()
    {
        yield return new WaitForSeconds(stunTime);
        stunStatusEffectActive = false;
        stunTime = 0;
    }
    IEnumerator slowDown()
    {
        agent.speed = chaseSpeed * slowModifier;

        yield return new WaitForSeconds(slowTime);
        agent.speed = chaseSpeed;
        slowStatusEffectActive = false;
        slowTime = 0;
    }
}

