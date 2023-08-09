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

    public List<GameObject> spawnedProjectiles = new List<GameObject>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();

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
            Projectile projectile = go.AddComponent<Projectile>();
            projectile.StartMoving();

            spawnedProjectiles.Add(go);
            yield return new WaitForSeconds(projectileSpawnParameters.spawnInterval);
        }
    }

    public void SpawnOnDeathProjectiles(Vector3 position)
    {
        float angleSegment = Mathf.PI / (GameSettings.instance.enemyProjectileDropCount - 1);

        for (int i = 0; i < GameSettings.instance.enemyProjectileDropCount; i++)
        {
            Vector3 spawnPosition = position;
            spawnPosition.x += GameSettings.instance.enemyProjectileDropRadius * Mathf.Cos(angleSegment * i);
            spawnPosition.z += GameSettings.instance.enemyProjectileDropRadius * Mathf.Sin(angleSegment * i);

            GameObject go = Instantiate(projectileSpawnParameters.prefab, spawnPosition, Quaternion.identity);
            spawnedProjectiles.Add(go);
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(enemySpawnParameters.spawnDelay);

        while (true)
        {
            GameObject go = Instantiate(enemySpawnParameters.prefab, GetRandomSpawnPoint(enemySpawnParameters.spawnPoints), Quaternion.identity);
            Enemy enemy = go.AddComponent<Enemy>();

            spawnedEnemies.Add(go);
            yield return new WaitForSeconds(enemySpawnParameters.spawnInterval);
        }
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
            float offset = transform.localScale.x / (projectileSpawnParameters.rowCount - 1);
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
            float offset = transform.localScale.x / (enemySpawnParameters.rowCount - 1);
            Vector3 point = transform.position;
            point.x -= offset * (enemySpawnParameters.rowCount / 2);
            point.z += .5f;

            enemySpawnParameters.spawnPoints = new Vector3[enemySpawnParameters.rowCount];
            for (int i = 0; i < enemySpawnParameters.rowCount; i++)
            {
                enemySpawnParameters.spawnPoints[i] = point;
                point.x += offset;
            }
        }
    }
}
