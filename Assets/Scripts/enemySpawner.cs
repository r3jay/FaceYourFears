using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemies;
    [SerializeField] int timeBetweenEnemies;

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
            Instantiate(enemy, transform.position, enemy.transform.rotation);
            yield return new WaitForSeconds(timeBetweenEnemies);
            isSpawning = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.enemyIncrement(maxEnemies);
            startSpawning = true;
        }
    }
}
