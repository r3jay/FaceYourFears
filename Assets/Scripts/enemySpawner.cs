using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> enemies;
    [SerializeField] int maxEnemies;
    [SerializeField] float timeBetweenEnemies;

    float startTime;
    [SerializeField] float timeToSpawnInSeconds;

    int enemiesSpawned;
    bool isSpawning;
    bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.enemyIncrement(maxEnemies);
        startTime = Time.time;
    }
    private void Update()
    {
        float timePassed = Time.time - startTime;
        if (timePassed >= timeToSpawnInSeconds)
        {
            startSpawning = true;
        }
        if (startSpawning)
        {
            StartCoroutine(Spawn());
        }
    }
    IEnumerator Spawn()
    {
        if (!isSpawning && enemiesSpawned < maxEnemies)
        {
            isSpawning = true;
            enemiesSpawned++;
            Quaternion randomAngle = new Quaternion(transform.rotation.x, Random.Range(0, 180), transform.rotation.z, transform.rotation.w);

            int rand = Random.Range(0, enemies.Count);
            Instantiate(enemies[rand], transform.position, randomAngle);
            yield return new WaitForSeconds(timeBetweenEnemies);
            isSpawning = false;
        }
    }



    //uncomment this to make spawners spawn enemies on player entering a range
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        startSpawning = true;
    //    }
    //}
}
