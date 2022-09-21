using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemies;
    [SerializeField] float timeBetweenEnemies;

    int enemiesSpawned;
    bool isSpawning;
    bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
    }
    private void Update()
    {
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
            Instantiate(enemy, transform.position, randomAngle);
            yield return new WaitForSeconds(timeBetweenEnemies);
            isSpawning = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
