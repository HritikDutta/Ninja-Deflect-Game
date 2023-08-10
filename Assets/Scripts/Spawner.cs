using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    [System.Serializable]
    public struct SpawnObjectParameters
    {
        public int rowCount;
        public GameObject prefab;
        public float spawnInterval;
        public float spawnDelay;
        public Vector3[] spawnPoints;
    }

    [SerializeField] private SpawnObjectParameters projectileSpawnParameters;
    [SerializeField] private SpawnObjectParameters enemySpawnParameters;
    [SerializeField] private GameObject pickUpPrefab;

    [HideInInspector] public List<GameObject> spawnedProjectiles = new List<GameObject>();
    [HideInInspector] public List<GameObject> spawnedEnemies = new List<GameObject>();

    private Coroutine projectileSpawnerCoroutine;
    private Coroutine enemySpawnerCoroutine;

    private void Awake()
    {
        // Only one instance allowed >:(
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SetupSpawnPoints();
        ResetSpawning();
    }

    IEnumerator SpawnProjectiles()
    {
        yield return new WaitForSeconds(projectileSpawnParameters.spawnDelay);

        while (true)
        {
            GameObject go = Instantiate(projectileSpawnParameters.prefab, GetRandomSpawnPoint(projectileSpawnParameters.spawnPoints), Quaternion.identity);
            go.GetComponent<Projectile>().Spawn();

            spawnedProjectiles.Add(go);
            yield return new WaitForSeconds(projectileSpawnParameters.spawnInterval);
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(enemySpawnParameters.spawnDelay);

        while (true)
        {
            // Hard limit so phone screen doesn't get crowded with enemies
            if (spawnedEnemies.Count >= GameSettings.instance.maxEnemiesOnScreen)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            GameObject go = Instantiate(enemySpawnParameters.prefab, GetRandomSpawnPoint(enemySpawnParameters.spawnPoints), Quaternion.identity);
            go.GetComponent<Enemy>().Spawn();

            spawnedEnemies.Add(go);
            yield return new WaitForSeconds(enemySpawnParameters.spawnInterval);
        }
    }

    public void SpawnProjectilesAroundPosition(Vector3 position)
    {
        for (int i = 0; i < GameSettings.instance.enemyProjectileDropCount; i++)
        {
            Vector3 spawnPosition = position;
            spawnPosition.z += GameSettings.instance.enemyProjectileDropGap * i;

            GameObject go = Instantiate(projectileSpawnParameters.prefab, spawnPosition, Quaternion.identity);
            go.GetComponent<Projectile>().Spawn();
            spawnedProjectiles.Add(go);
        }
    }

    public void SpawnPickUp(Vector3 position)
    {
        GameObject go = Instantiate(pickUpPrefab, position, Quaternion.identity);
        go.GetComponent<PickUp>().Spawn();
    }

    private Vector3 GetRandomSpawnPoint(Vector3[] spawnPoints)
    {
        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (Vector3 point in projectileSpawnParameters.spawnPoints)
        {
            Gizmos.DrawWireSphere(point, 0.02f);
        }

        Gizmos.color = Color.yellow;
        foreach (Vector3 point in enemySpawnParameters.spawnPoints)
        {
            Gizmos.DrawWireSphere(point, 0.1f);
        }
    }

    public void StopSpawning()
    {
        StopCoroutine(projectileSpawnerCoroutine);
        StopCoroutine(enemySpawnerCoroutine);
    }

    public void DespawnEnemy(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }

    public void DespawnProjectile(GameObject projectile)
    {
        spawnedProjectiles.Remove(projectile);
    }

    public void ResetSpawning()
    {
        {   // Projectiles
            foreach (GameObject projectile in spawnedProjectiles)
                Destroy(projectile);

            spawnedProjectiles.Clear();
        }

        {   // Enemies
            foreach (GameObject enemy in spawnedEnemies)
                Destroy(enemy);

            spawnedEnemies.Clear();
        }

        projectileSpawnerCoroutine = StartCoroutine(SpawnProjectiles());
        enemySpawnerCoroutine      = StartCoroutine(SpawnEnemies());
    }

    [ContextMenu("Setup Spawn Points")]
    void SetupSpawnPoints()
    {
        {   // Projectiles
            float offset = projectileSpawnParameters.rowCount > 1 ? transform.localScale.x / (projectileSpawnParameters.rowCount - 1) : 0f;
            Vector3 point = transform.position;
            point.x -= offset * (projectileSpawnParameters.rowCount / 2);

            projectileSpawnParameters.spawnPoints = new Vector3[projectileSpawnParameters.rowCount];
            for (int i = 0; i < projectileSpawnParameters.rowCount; i++)
            {
                projectileSpawnParameters.spawnPoints[i] = point;
                point.x += offset;
            }
        }

        {   // Enemies
            float offset = enemySpawnParameters.rowCount > 1 ? transform.localScale.x / (enemySpawnParameters.rowCount - 1) : 0f;
            Vector3 point = transform.position;
            point.x -= offset * (enemySpawnParameters.rowCount / 2);
            //point.z += .5f;

            enemySpawnParameters.spawnPoints = new Vector3[enemySpawnParameters.rowCount];
            for (int i = 0; i < enemySpawnParameters.rowCount; i++)
            {
                enemySpawnParameters.spawnPoints[i] = point;
                point.x += offset;
            }
        }
    }
}
