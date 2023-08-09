using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageDealer
{
    [SerializeField] private LayerMask projectileLayerMask;

    private List<Projectile> deflectedProjectiles = new List<Projectile>();

    private TextMeshPro healthText;
    private int health;

    private void Awake()
    {
        health = GameSettings.instance.enemyMaxHealth;

        healthText = GetComponentInChildren<TextMeshPro>();
        healthText.text = health.ToString();
    }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & projectileLayerMask) == 0)
            return;

        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        // Only take damage from deflected projectiles
        if (!projectile.deflected)
            return;

        deflectedProjectiles.Remove(projectile);
        //Destroy(collision.gameObject);

        health--;
        healthText.text = health.ToString();

        if (health <= 0)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Spawner.instance.DespawnEnemy(gameObject);
    }

    public float Damage => GameSettings.instance.enemyParameters.damage;
}
