using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageDealer
{
    [SerializeField] private LayerMask projectileLayerMask;

    private List<Projectile> deflectedProjectiles = new List<Projectile>();

    private TextMeshPro healthText;
    private int health;

    HashSet<Projectile> hitProjectiles = new HashSet<Projectile>();

    private void Update()
    {
        transform.position += GameSettings.instance.enemyParameters.moveSpeed * Time.deltaTime * Vector3.back;

        foreach (Projectile projectile in deflectedProjectiles)
        {
            projectile.moveDirection = (transform.position - projectile.transform.position).normalized;
        }
    }

    public void AddDeflectedProjectile(Projectile projectile)
    {
        deflectedProjectiles.Add(projectile);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & projectileLayerMask) == 0)
            return;

        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        // Only take damage from deflected projectiles
        if (!projectile.deflected)
        {
            projectile.Despawn();
            return;
        }

        // Don't take damage from the same projectile more than once!
        if (hitProjectiles.Contains(projectile))
            return;

        deflectedProjectiles.Remove(projectile);
        hitProjectiles.Add(projectile);

        health--;
        healthText.text = health.ToString();

        if (health <= 0)
            Despawn();
    }
    public void Spawn()
    {
        health = GameSettings.instance.enemyMaxHealth;

        healthText = GetComponentInChildren<TextMeshPro>();
        healthText.text = health.ToString();
    }

    public void Despawn()
    {
        if (Random.Range(0f, 1f) <= GameSettings.instance.enemyProjectileDropChance)
            Spawner.instance.SpawnProjectilesAroundPosition(transform.position);

        Spawner.instance.DespawnEnemy(gameObject);
        Destroy(gameObject);
    }

    public float Damage => GameSettings.instance.enemyParameters.damage;
}
