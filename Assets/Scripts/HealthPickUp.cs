using UnityEngine;

public class HealthPickUp : MonoBehaviour, ISpawnObject
{
    [SerializeField]
    private Transform visualTransform;

    [Header("Spawn Animation")]
    [SerializeField]
    private float spawnHeightOffset = 0.1f;
    [SerializeField]
    private float fallSpeed = 0.1f;

    private bool collected = false;

    private Vector3 spawnPosition;

    public void Spawn()
    {
        collected = false;
        spawnPosition = visualTransform.position;

        {   // Visual Transform
            Vector3 position = visualTransform.position;
            position.y += spawnHeightOffset;
            visualTransform.position = position;
        }
    }

    public void Despawn()
    {
        Spawner.instance.DespawnPickup(gameObject);
        TownCollider.instance.AddOrReduceHealth(GameSettings.instance.healthPickupHealAmount);
        Destroy(gameObject);
    }

    private void Update()
    {
        visualTransform.Rotate(0f, 600f * Time.deltaTime, 0f);

        if (Mathf.Abs(visualTransform.position.y - spawnPosition.y) >= 0.001f)
        {
            float newYPosition = Mathf.Max(visualTransform.position.y - fallSpeed * Time.deltaTime, spawnPosition.y);

            {   // Visual Transform
                Vector3 position = visualTransform.position;
                position.y = newYPosition;
                visualTransform.position = position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        // No need to check layer since it only interacts with the player
        visualTransform.gameObject.SetActive(false);

        collected = true;
        Despawn();
    }
}
