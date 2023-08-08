using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    }

    [System.Serializable]
    public struct SpawnObjectBehvaiour
    {
        public float damage;
        public float moveSpeed;
    }

    [SerializeField] private SpawnObjectParameters projectileSpawnParameters;
    [SerializeField] private SpawnObjectBehvaiour  projectileBehaviour;

    [SerializeField] private SpawnObjectParameters enemySpawnParameters;
    [SerializeField] private SpawnObjectBehvaiour  enemyBehaviour;

    [SerializeField] private int enemyMaxHealth = 1;

    private List<GameObject> spawnedProjectiles = new List<GameObject>();

    private List<TextMeshPro> enemyHealthTexts = new List<TextMeshPro>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<int> enemyHealths = new List<int>();

    private Coroutine projectileSpawnerCoroutine;
    private Coroutine enemySpawnerCoroutine;

    private void Start()
    {
        SetupSpawnPoints();
        ResetSpawning();
    }

    void Update()
    {
        {   // Projectiles
            Vector3 movement = projectileBehaviour.moveSpeed * Time.deltaTime * Vector3.back;
            foreach (GameObject projectile in spawnedProjectiles)
            {
                projectile.transform.position += movement;
            }
        }

        {   // Projectiles
            Vector3 movement = enemyBehaviour.moveSpeed * Time.deltaTime * Vector3.back;
            foreach (GameObject enemy in spawnedEnemies)
            {
                enemy.transform.position += movement;
            }
        }
    }

    IEnumerator SpawnProjectiles()
    {
        while (true)
        {
            spawnedProjectiles.Add(Instantiate(projectileSpawnParameters.prefab, GetRandomSpawnPoint(projectileSpawnParameters.spawnPoints), Quaternion.identity));
            yield return new WaitForSeconds(projectileSpawnParameters.spawnInterval);
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            GameObject enemy = Instantiate(enemySpawnParameters.prefab, GetRandomSpawnPoint(enemySpawnParameters.spawnPoints), Quaternion.identity);
            TextMeshPro healthText = enemy.GetComponentInChildren<TextMeshPro>();
            healthText.text = enemyMaxHealth.ToString();

            enemyHealths.Add(enemyMaxHealth);
            enemyHealthTexts.Add(healthText);
            spawnedEnemies.Add(enemy);

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

    // Pls don't give some other object :(
    public void DespawnObject(GameObject gameObject, out float damage)
    {
        damage = 0f;

        if (spawnedProjectiles.Remove(gameObject))
            damage = projectileBehaviour.damage;
        else
        {
            int enemyIndex = spawnedEnemies.FindIndex((GameObject o) => o == gameObject);
            if (enemyIndex < 0)
                Debug.LogError($"Couldn't identify the object {gameObject} as a projectile or an enemy!");

            spawnedEnemies.RemoveAt(enemyIndex);
            enemyHealths.RemoveAt(enemyIndex);
            enemyHealthTexts.RemoveAt(enemyIndex);
            damage = enemyBehaviour.damage;
        }

        Destroy(gameObject);
    }

    public void DeflectProjectile(GameObject gameObject)
    {
        // TODO: Snap the projectile to the target
        spawnedProjectiles.Remove(gameObject);
        Destroy(gameObject);

        if (spawnedEnemies.Count <= 0)
            return;

        // The first enemy in the list would be the nearest to the town
        enemyHealths[0]--;
        enemyHealthTexts[0].text = enemyHealths[0].ToString();

        if (enemyHealths[0] <= 0)
        {
            Destroy(spawnedEnemies[0]);

            spawnedEnemies.RemoveAt(0);
            enemyHealths.RemoveAt(0);
            enemyHealthTexts.RemoveAt(0);
        }
    }

    public void StopSpawning()
    {
        StopCoroutine(projectileSpawnerCoroutine);
        StopCoroutine(enemySpawnerCoroutine);
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
            enemyHealths.Clear();
            enemyHealthTexts.Clear();
        }

        projectileSpawnerCoroutine = StartCoroutine(SpawnProjectiles());
        enemySpawnerCoroutine      = StartCoroutine(SpawnEnemies());
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
