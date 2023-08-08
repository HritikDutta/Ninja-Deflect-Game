using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnObjectParameters
    {
        public int rowCount;
        public GameObject prefab;
        public float spawnInterval;
        public Vector3[] spawnPoints;
        public float moveSpeed;
    }

    [SerializeField] private SpawnObjectParameters projectileSpawnParameters;
    [SerializeField] private SpawnObjectParameters enemySpawnParameters;

    private bool keepSpawning = true;
    private List<GameObject> spawnedProjectiles = new List<GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnProjectiles());
        StartCoroutine(SpawnEnemies());
        SetupSpawnPoints();
    }

    void Update()
    {
        {   // Projectiles
            Vector3 movement = projectileSpawnParameters.moveSpeed * Time.deltaTime * Vector3.back;
            foreach (GameObject projectile in spawnedProjectiles)
            {
                projectile.transform.position += movement;
            }
        }

        {   // Projectiles
            Vector3 movement = enemySpawnParameters.moveSpeed * Time.deltaTime * Vector3.back;
            foreach (GameObject enemy in spawnedEnemies)
            {
                enemy.transform.position += movement;
            }
        }
    }

    IEnumerator SpawnProjectiles()
    {
        while (keepSpawning)
        {
            spawnedProjectiles.Add(Instantiate(projectileSpawnParameters.prefab, GetRandomSpawnPoint(projectileSpawnParameters.spawnPoints), Quaternion.identity));
            yield return new WaitForSeconds(projectileSpawnParameters.spawnInterval);
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (keepSpawning)
        {
            spawnedEnemies.Add(Instantiate(enemySpawnParameters.prefab, GetRandomSpawnPoint(enemySpawnParameters.spawnPoints), Quaternion.identity));
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

    public void DespawnObject(GameObject gameObject)
    {
        if (spawnedProjectiles.Remove(gameObject) || spawnedEnemies.Remove(gameObject))
            Destroy(gameObject);
    }

    [ContextMenu("Setup Spawn Points")]
    void SetupSpawnPoints()
    {
        {   // Projectiles
            float offset = transform.localScale.x / projectileSpawnParameters.rowCount;
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
            float offset = transform.localScale.x / enemySpawnParameters.rowCount;
            Vector3 point = transform.position;
            point.x -= offset * (enemySpawnParameters.rowCount / 2);

            enemySpawnParameters.spawnPoints = new Vector3[enemySpawnParameters.rowCount];
            for (int i = 0; i < enemySpawnParameters.rowCount; i++)
            {
                enemySpawnParameters.spawnPoints[i] = point;
                point.x += offset;
            }
        }
    }
}
