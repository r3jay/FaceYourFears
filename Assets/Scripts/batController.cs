using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class batController : MonoBehaviour
{

    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    public Renderer rend;
    [SerializeField] Animator animator;

    [Header("----- Enemy Stats -----")]
    [Range(1, 100)] public int HP;
    [SerializeField] public float chaseSpeed;
    [Range(1, 50)] [SerializeField] int roamRadius;
    [Range(1, 180)] [SerializeField] int fovAngle;
    [Range(1, 10)] [SerializeField] int playerFaceSpeed;
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
    bool isAttacking;

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
    [SerializeField] private Vector3 lootLocation;


    Vector3 playerDir;
    Vector3 startingPosition;
    bool playerInRange;
    bool playerSeen;
    Vector3 lastPlayerPos;
    //bool searchingForPlayer;
    float speedOrig;
    int randomNumber;

    public bool takingDamage;
    GameObject ground;

    [HideInInspector] public bool stunStatusEffectActive;
    [HideInInspector] public float stunTime;
    [HideInInspector] public bool slowStatusEffectActive;
    [Range(0, 1)] public float slowModifier;
    public float slowTime;


    // Start is called before the first frame update
    void Start()
    {
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

        flyingNearby = false;
        agent.SetDestination(gameManager.instance.player.transform.position);
        ground = GameObject.FindGameObjectWithTag("Terrain");
    }

    // Update is called once per frame
    void Update()
    {

        // Only run through update if the enemy is enabled ie alive
        if (agent.enabled == true)
        {
            // find direction to player
            playerDir = gameManager.instance.player.transform.position - transform.position;

            if (!animator.GetBool("Dead"))
            {
                animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));
            }

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
                            if (!isAttacking)
                            {
                                if (agent.remainingDistance <= meleeStoppingDistance)
                                {
                                    StartCoroutine(meleeAttack());
                                }
                            }
                        }
                        else
                        {
                            if (!attackTimerRunning)
                            {
                                StartCoroutine(randomAttackTimer());
                            }
                            if (agent.remainingDistance <= agent.stoppingDistance)
                            {
                                createNewPath();
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
        else if (fallToGroundAfterDeathAnimation() != Vector3.zero)
        {
            {
                transform.position = fallToGroundAfterDeathAnimation();
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
            GetComponentInChildren<batMeleeWeaponController>().GetComponent<Collider>().enabled = false;
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
        isAttacking = false;
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
    public Vector3 fallToGroundAfterDeathAnimation()
    {
        if (ground && transform.position.y >= ground.transform.position.y + .01f)
        {
            Vector3 fallPos = new Vector3(transform.position.x, ground.transform.position.y, transform.position.z);
            return Vector3.Lerp(transform.position, fallPos, Time.deltaTime * 1);
        }
        else
        {
            return Vector3.zero;
        }
    }
}

