using UnityEngine;

public class TownCollider : MonoBehaviour
{
    [SerializeField] private LayerMask damageLayers;
    [SerializeField] private Spawner spawner;

    [SerializeField] private float maxHealth = 100;

    private float health = 0f;

    private void Start()
    {
        ResetTown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageLayers) == 0)
            return;

        spawner.DespawnObject(other.gameObject, out float damage);
        health -= damage;

        if (health <= 0f)
        {
            UIController.instance.gameOverScreen.SetActive(true);
            spawner.StopSpawning();
        }
    }

    public void ResetTown()
    {
        health = maxHealth;
    }
}
