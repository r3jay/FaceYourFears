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
    [Range(1, 10)] [SerializeField] float chaseSpeed;
    [Range(1, 50)] [SerializeField] int roamRadius;
    [Range(1, 180)] [SerializeField] int fovAngle;
    [Range(1, 10)] [SerializeField] int playerFaceSpeed;
    [SerializeField] float followDistance;
    [SerializeField] float stoppingDistance;
    bool isTargetingHouse;
    Transform houseTarget;
    public bool houseInRange;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    bool isShooting;

    public bool isInMeleeRange;
    [SerializeField] GameObject meleeWeaponLeft;
    [SerializeField] GameObject meleeWeaponRight;
    [SerializeField] bool isMeleeAttacker;
    bool isAttacking;

    [SerializeField] List<boostPickUp> boost = new List<boostPickUp>();
    [SerializeField] List<keyPickUp> key = new List<keyPickUp>();
    [SerializeField] private Vector3 lootLocation;


    Vector3 playerDir;
    Vector3 startingPosition;
    bool playerInRange;
    bool playerSeen;
    Vector3 lastPlayerPos;
    bool searchingForPlayer;
    float speedOrig;
    int randomNumber;

    bool takingDamage;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = followDistance;
        isTargetingHouse = true;
        if (gameManager.instance.houseTarget.GetComponent<baseController>().targetPositions.Count != 0)
        {
            int rand = Random.Range(0, gameManager.instance.houseTarget.GetComponent<baseController>().targetPositions.Count - 1);
            houseTarget = gameManager.instance.houseTarget.GetComponent<baseController>().targetPositions[rand].transform;
        }
        lastPlayerPos = transform.position;
        speedOrig = agent.speed;
        if (isMeleeAttacker)
        {
            stoppingDistance = 2.5f;
        }
        agent.stoppingDistance = stoppingDistance;
        startingPosition = transform.position;
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
                if (isTargetingHouse)
                {
                    moveToTarget();
                    faceTarget();
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
                if (playerInRange && !isTargetingHouse)
                {
                    if (canSeePlayer())
                    {
                        facePlayer();
                        chasePlayer();
                        if (!isShooting && !isMeleeAttacker)
                        {
                            StartCoroutine(shoot());
                        }
                        else if (!isAttacking && isMeleeAttacker && isInMeleeRange)
                        {
                            StartCoroutine(meleeAttack());
                        }
                    }
                    // player within range but not in view
                    else if (Vector3.Angle(playerDir, transform.forward) > fovAngle && !playerSeen)// NOTE :: without a high turning rate, enemies are dumb af. This line should maybe change.
                                                                                                   // Going behind them might as well be the same as teleporting 100 miles away.
                    {
                        agent.stoppingDistance = 0;
                    }
                }
                // Search for the last known player position
                else if (!playerInRange && searchingForPlayer && !isTargetingHouse)
                {
                    agent.SetDestination(lastPlayerPos);
                    agent.stoppingDistance = 0;
                }
                // if enemy gets to lastPlayerPosition starting roaming from a new location
                if (agent.remainingDistance < 0.1f && searchingForPlayer && !isTargetingHouse)
                {
                    searchingForPlayer = false;
                    startingPosition = transform.position;
                    roam();
                }
                // default behavior
                else if (agent.remainingDistance < 0.1f && !searchingForPlayer && !isTargetingHouse)
                {
                    roam();
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
        agent.stoppingDistance = stoppingDistance;
        Vector3 targetVector = new Vector3(houseTarget.position.x, houseTarget.position.y, houseTarget.position.z);
        agent.SetDestination(targetVector);
    }

    void faceTarget()
    {
        playerDir.y = 0;
        Vector3 targetDir = new Vector3(houseTarget.transform.position.x, houseTarget.transform.position.y, houseTarget.transform.position.z) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(targetDir); //location we want the enemy to look toward
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    // Check to see if player is within view - return true if player is seen
    bool canSeePlayer()
    {
        float angle = Vector3.Angle(playerDir, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, playerDir, out hit))
        {
            Debug.DrawRay(transform.position + transform.up, playerDir);

            // if the raycast hits a player and is within enemies FOV
            if (hit.collider.CompareTag("Player") && angle <= fovAngle)
            {
                playerSeen = true;
                searchingForPlayer = false;
                lastPlayerPos = gameManager.instance.player.transform.position;
                return true;
            }
            else
            {
                playerSeen = false;
                searchingForPlayer = true;
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
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(lastPlayerPos);
    }

    //get enemy to face the player
    void facePlayer()
    {
        //// if we want the option to have the enemy aim up and down, uncomment these two lines
        //Quaternion aimRotation = Quaternion.LookRotation(playerDir);
        //shootPos.transform.rotation = Quaternion.Lerp(shootPos.transform.rotation, aimRotation, Time.deltaTime * playerFaceSpeed);

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

            // update lastPlayerPos so enemy investigates where it was shot from
            lastPlayerPos = gameManager.instance.player.transform.position;
            searchingForPlayer = true;
            agent.SetDestination(lastPlayerPos);
            agent.stoppingDistance = 0;

            StartCoroutine(flashColor());

            if (HP <= 0)
            {
                enemyDead();
            }
        }


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
        Debug.Log("agent disabled");


        //// after death, delete colliders... currently worked so takeDamage just does nothing so that enemy bodies can still be interacted with
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }

    }

    IEnumerator flashColor() //changes the color of the enemy.
    {
        takingDamage = true;
        float tempSpeed = agent.speed;
        agent.speed = 0;

        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;

        yield return new WaitForSeconds(0.4f);
        agent.speed = tempSpeed;
        takingDamage = false;
    }

    IEnumerator shoot()
    {
        float tempSpeed = agent.speed;
        agent.speed = 0;
        isShooting = true;
        animator.SetTrigger("Attack");
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
        float tempSpeed = agent.speed;
        agent.speed = 0;
        isAttacking = true;
        animator.SetTrigger("Attack");
        if (meleeWeaponLeft)
        {
            meleeWeaponLeft.GetComponent<Collider>().enabled = true;
        }
        if (meleeWeaponRight)
        {
            meleeWeaponRight.GetComponent<Collider>().enabled = true;
        }
        yield return new WaitForSeconds(1);
        agent.speed = tempSpeed;
        if (meleeWeaponLeft)
        {
            meleeWeaponLeft.GetComponent<Collider>().enabled = false;
        }
        if (meleeWeaponRight)
        {
            meleeWeaponRight.GetComponent<Collider>().enabled = false;
        }
        yield return new WaitForSeconds(shootRate);

        isAttacking = false;
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
            if (playerSeen)
            {
                lastPlayerPos = gameManager.instance.player.transform.position;
                searchingForPlayer = true;
                playerSeen = false;
            }

        }
    }

}
