using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageDealer
{
    [SerializeField] private LayerMask projectileLayerMask;
    [SerializeField] private ParticleSystem damageParticleEffect;

    [SerializeField] private Transform[] bodyPartsToBlowOff;

    private List<Projectile> deflectedProjectiles = new List<Projectile>();

    private TextMeshPro healthText;
    private Animator animator;
    private Collider myCollider;
    private int health;

    HashSet<Projectile> hitProjectiles = new HashSet<Projectile>();
    private bool despawned = false;

    private void Update()
    {
        if (despawned)
            return;

        transform.position += GameSettings.instance.enemyParameters.moveSpeed * Time.deltaTime * Vector3.back;

        foreach (Projectile projectile in deflectedProjectiles)
        {
            float distance = (transform.position - projectile.transform.position).magnitude;
            if (distance >= 0.1f)
                projectile.moveDirection = (transform.position - projectile.transform.position) / distance;
        }
    }

    public void AddDeflectedProjectile(Projectile projectile)
    {
        deflectedProjectiles.Add(projectile);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (despawned)
            return;

        if (((1 << collision.gameObject.layer) & projectileLayerMask) == 0)
            return;

        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        // Only take damage from deflected projectiles
        if (!projectile.deflected)
        {
            projectile.Despawn(false);
            return;
        }

        // Don't take damage from the same projectile more than once!
        if (hitProjectiles.Contains(projectile))
            return;

        deflectedProjectiles.Remove(projectile);
        hitProjectiles.Add(projectile);

        health--;
        healthText.text = health.ToString();

        damageParticleEffect.Play();
        AudioController.PlayAudioClipOneShot(AudioController.instance.knifeStabClip);

        if (health <= 0)
            Despawn(true);
    }

    public void Spawn()
    {
        health = GameSettings.instance.enemyMaxHealth;

        healthText = GetComponentInChildren<TextMeshPro>();
        healthText.text = health.ToString();

        animator = GetComponentInChildren<Animator>();
        myCollider = GetComponent<Collider>();

        despawned = false;
    }

    public void Despawn(bool isDeath)
    {
        Spawner.instance.DespawnEnemy(gameObject);
        despawned = true;

        if (isDeath)
            health = 0;

        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        healthText.gameObject.SetActive(false);

        if (health <= 0)
        {
            animator.SetTrigger("Death");
            AudioController.PlayAudioClipOneShot(AudioController.instance.ogreDeath);
        }
        else
        {
            animator.SetTrigger("Attack");  // Enemy has probably reached the town
            AudioController.PlayAudioClipOneShot(AudioController.instance.ogreAttack);
        }

        myCollider.enabled = false;

        if (health <= 0 && Random.Range(0f, 1f) <= GameSettings.instance.enemyProjectileDropChance)
            Spawner.instance.SpawnExtraProjectiles(transform.position);

        Spawner.instance.SpawnPickUp(transform.position);

        yield return new WaitForSeconds(2.3f);
        Destroy(gameObject);
    }

    public float Damage => GameSettings.instance.enemyParameters.damage;
}
