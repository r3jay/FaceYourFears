using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamageable
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
    [Range(1, 10)] [SerializeField] float playerFaceSpeed;
    float playerFaceSpeedOrig;
    [SerializeField] float followDistance;
    [SerializeField] float rangedStoppingDistance;
    [SerializeField] float meleeStoppingDistance;
    public bool targetHouse;
    Transform houseTarget;
    public bool targetPlayer;
    public bool houseInRange;

    [Header("----- Weapon Stats -----")]
    public float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    bool isShooting;


    public int meleeDamage;
    public bool isInMeleeRange;
    public List<GameObject> meleeWeapons;
    public bool isMeleeAttacker;
    public bool dealsPosionDamage;
    public int poisonDamage;
    public float timeBetweenPoisonTicks;
    public float poisonTime;
    [SerializeField] bool hasMultipleAttacks;
    [SerializeField] int numberOfAttacks;
    bool isAttacking;
    public bool isTreant;
    public bool isLich;
    [SerializeField] float handSpawnRadiusFromPlayer;
    [SerializeField] bool isSpearmen;
    [SerializeField] float defendTime;
    bool defendTimerRunning;
    bool isDefending;
    [SerializeField] int takeDamageOverAngle;
    [Range(0, 1)] [SerializeField] float slowRotationModifier;

    [SerializeField] List<boostPickUp> boost = new List<boostPickUp>();
    [SerializeField] List<keyPickUp> key = new List<keyPickUp>();
    [SerializeField] private Vector3 lootLocation;


    Vector3 playerDir;
    Vector3 startingPosition;
    bool playerInRange;
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
        playerFaceSpeedOrig = playerFaceSpeed;
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

        // treant spawn check
        if (isTreant || isLich)
        {
            StartCoroutine(spawnPause());
        }

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
                playerInRange = false;
                agent.stoppingDistance = 0;
            }

            // checks for knockback animation
            if (!takingDamage)
            {
                if (targetHouse)
                {
                    if (slowStatusEffectActive)
                    {
                        StartCoroutine(slowDown());
                    }
                    if (!stunStatusEffectActive)
                    {
                        moveToTarget();
                        faceTarget();
                        if (!isShooting && isTreant && !houseInRange)
                        {
                            StartCoroutine(shoot());
                        }
                    }
                    else
                    {
                        StartCoroutine(stunTimer());
                    }
                    if (houseInRange)
                    {
                        if (agent.remainingDistance <= agent.stoppingDistance)
                        {
                            if (!isShooting && !isMeleeAttacker)
                            {
                                StartCoroutine(shoot());
                            }
                            else if (!isAttacking && isMeleeAttacker && isInMeleeRange)
                            {
                                StartCoroutine(meleeAttack());
                            }
                        }

                    }
                }
                else if (targetPlayer)
                {

                    if (slowStatusEffectActive)
                    {
                        StartCoroutine(slowDown());
                    }
                    if (!stunStatusEffectActive)
                    {
                        moveToPlayer();
                        facePlayer();
                    }
                    else
                    {
                        StartCoroutine(stunTimer());
                    }

                    if (playerInRange)
                    {
                        if (!isShooting && !isMeleeAttacker)
                        {
                            if (!isLich)
                            {
                                StartCoroutine(shoot());
                            }
                            else
                            {
                                StartCoroutine(lichSpawnHands());
                            }
                        }
                        else if (!isAttacking && isMeleeAttacker && isInMeleeRange && !isDefending)
                        {
                            StartCoroutine(meleeAttack());
                        }
                    }
                }
            }
        }
    }

    // When player is not seen, roam around a set area
    void roam()
    {
        agent.stoppingDistance = 0; // guarantees enemy will stop exactly at location
        agent.speed = speedOrig;

        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += startingPosition;

        NavMeshHit hit;
        // Checks to see if path is valid, sets hit if true
        NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);
    }

    void moveToTarget()
    {
        if (isMeleeAttacker)
        {
            agent.stoppingDistance = meleeStoppingDistance;
        }
        else
        {
            agent.stoppingDistance = rangedStoppingDistance;
        }
        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(houseTarget.position.x, houseTarget.position.y, houseTarget.position.z), out hit, roamRadius, 1);
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);

        if (takingDamage)
        {
            agent.speed = 0;
        }
        else
        {
            agent.speed = chaseSpeed;
        }
    }

    void faceTarget()
    {
        playerDir.y = 0;
        Vector3 targetDir = new Vector3(houseTarget.transform.position.x, houseTarget.transform.position.y, houseTarget.transform.position.z) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(targetDir); //location we want the enemy to look toward
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    void moveToPlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
    }

    // Check to see if player is within view - return true if player is seen
    bool canSeePlayer()
    {
        float angle = Vector3.Angle(playerDir, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward, playerDir, out hit))
        {
            Debug.DrawRay(transform.position + transform.forward, playerDir);

            // if the raycast hits a player and is within enemies FOV
            if (hit.collider.CompareTag("Player") && angle <= fovAngle)
            {
                //searchingForPlayer = false;
                lastPlayerPos = gameManager.instance.player.transform.position;
                return true;
            }
            else
            {
                //searchingForPlayer = true;
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // chasing behaviour
    void chasePlayer()
    {
        if (isMeleeAttacker)
        {
            agent.stoppingDistance = meleeStoppingDistance;
        }
        else
        {
            agent.stoppingDistance = rangedStoppingDistance;
        }
        agent.SetDestination(lastPlayerPos);
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
        if (!isSpearmen || (isSpearmen && isAttacking))
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
        else
        {
            if (agent.enabled)
            {
                float angle = Vector3.Angle(playerDir, transform.forward);
                if (angle < takeDamageOverAngle)
                {
                    isDefending = true;
                    animator.SetBool("Defending", true);
                    agent.velocity = Vector3.zero;
                    agent.isStopped = true;
                    playerFaceSpeed *= slowRotationModifier;
                    if (!defendTimerRunning)
                    {
                        StartCoroutine(defendTimer());
                    }

                }
                else
                {
                    isDefending = false;
                    animator.SetBool("Defending", false);
                    HP -= dmg;
                    animator.SetTrigger("Damage");
                    playerFaceSpeed = playerFaceSpeedOrig;
                    agent.speed = chaseSpeed;
                    agent.isStopped = false;
                }
            }
        }
    }

    IEnumerator defendTimer()
    {
        defendTimerRunning = true;
        yield return new WaitForSeconds(defendTime);
        defendTimerRunning = false;
        isDefending = false;
        animator.SetBool("Defending", false);
        playerFaceSpeed = playerFaceSpeedOrig;
        agent.speed = chaseSpeed;
        agent.isStopped = false;
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

    IEnumerator spawnPause()
    {
        isShooting = true;
        float rand = Random.Range(1, shootRate + 3);
        yield return new WaitForSeconds(rand);
        isShooting = false;
    }
    IEnumerator lichSpawnHands()
    {
        float tempSpeed = agent.speed;
        agent.speed = 0;
        isShooting = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(.5f);

        Vector3 randomPosition = gameManager.instance.player.transform.position + Random.insideUnitSphere * handSpawnRadiusFromPlayer;
        randomPosition.y = 0;
        Instantiate(bullet, randomPosition, transform.localRotation);
        // wait for attack animation to stop before reseeting speed to normal
        yield return new WaitForSeconds(.5f);
        agent.speed = tempSpeed;
        //Debug.Log("Enemy fired");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    IEnumerator shoot()
    {
        float tempSpeed = agent.speed;
        agent.speed = 0;
        isShooting = true;
        if (!isTreant)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            animator.SetTrigger("Projectile");
        }
        yield return new WaitForSeconds(.5f);
        Instantiate(bullet, shootPos.transform.position, transform.rotation); //when enemy shoots, instantiate the bullet where enemy is located, in the bullets rotation
        // wait for attack animation to stop before reseeting speed to normal
        yield return new WaitForSeconds(.5f);
        agent.speed = tempSpeed;
        //Debug.Log("Enemy fired");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator meleeAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        isAttacking = true;
        if (hasMultipleAttacks)
        {
            animator.SetTrigger("Attack" + Random.Range(1, numberOfAttacks + 1));
        }
        else
        {
            animator.SetTrigger("Attack");
        }

        // attack build up
        yield return new WaitForSeconds(.5f);

        if (!animator.GetBool("Dead"))
        {
            foreach (GameObject weapon in meleeWeapons)
            {
                weapon.GetComponent<Collider>().enabled = true;
            }
        }


        // wait for attack animation to finish
        yield return new WaitForSeconds(1);

        // if an attack misses, still turn off colliders after full attack
        foreach (GameObject weapon in meleeWeapons)
        {
            weapon.GetComponent<Collider>().enabled = false;
        }

        if (!animator.GetBool("Dead"))
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;

            yield return new WaitForSeconds(shootRate);

            isAttacking = false;
        }

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

