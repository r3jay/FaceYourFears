using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamageable
{

    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] GameObject destination;

    [Header("----- Enemy Stats -----")]
    [Range(0, 10)] [SerializeField] int HP;
    [Range(1, 10)] [SerializeField] int playerFaceSpeed;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] float shootDist;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    Vector3 playerDir;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.enemyIncrement();

    }

    // Update is called once per frame
    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;  //(position - position = direction)

        agent.SetDestination(gameManager.instance.player.transform.position); //get the enemy to go to a point in the level. (player)

        if (Vector3.Distance(transform.position, gameManager.instance.player.transform.position) < shootDist) //checking the vector distance of player, getting position, checking if within shooting distance.
        {

            if (!isShooting)
                StartCoroutine(shoot());
            facePlayer();

        }


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
        HP -= dmg;
        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            gameManager.instance.enemyDecrement();
            Destroy(gameObject);
        }

    }

    IEnumerator flashColor() //changes the color of the enemy.
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation); //when enemy shoots, instantiate the bullet where enemy is located, in the bullets rotation
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
