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

    [Header("----- Weapon Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    bool isShooting;

    Vector3 playerDir;
    Vector3 startingPosition;
    bool playerInRange;
    bool playerSeen;
    Vector3 lastPlayerPos;
    bool searchingForPlayer;
    float speedOrig;
    float stoppingDistanceOrig;

    bool takingDamage;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = followDistance;
        lastPlayerPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
        speedOrig = agent.speed;
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Only run through update if the enemy is enabled ie alive
        if (agent.enabled)
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
                if (playerInRange)
                {
                    if (canSeePlayer())
                    {
                        facePlayer();
                        chasePlayer();
                        if (!isShooting)
                        {
                            StartCoroutine(shoot());
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
                else if (!playerInRange && searchingForPlayer)
                {
                    agent.SetDestination(lastPlayerPos);
                    agent.stoppingDistance = 0;
                }
                // if enemy gets to lastPlayerPosition starting roaming from a new location
                if (agent.remainingDistance < 0.1f && searchingForPlayer)
                {
                    searchingForPlayer = false;
                    startingPosition = transform.position;
                    roam();
                }
                // default behavior
                else if (agent.remainingDistance < 0.1f && !searchingForPlayer)
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
        agent.stoppingDistance = stoppingDistanceOrig;
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
        agent.enabled = false;

        //// after death, delete colliders... currently worked so takeDamage just does nothing so that enemy bodies can still be interacted with
        //foreach(Collider col in GetComponents<Collider>())
        //{
        //    col.enabled = false;
        //}

    }

    IEnumerator flashColor() //changes the color of the enemy.
    {
        takingDamage = true;
        float tempSpeed = agent.speed;
        agent.speed = 0;
        Material tempMat = rend.material;
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        rend.material = tempMat;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(.5f);
        Instantiate(bullet, shootPos.transform.position, transform.rotation); //when enemy shoots, instantiate the bullet where enemy is located, in the bullets rotation
        Debug.Log("Enemy fired");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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
