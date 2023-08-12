using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ISpawnObject
{
    [SerializeField] private LayerMask projectileLayerMask;
    [SerializeField] private ParticleSystem damageParticleEffect;

    [SerializeField] private List<EnemyUnit> units = new List<EnemyUnit>();

    private Collider myCollider;
    private Rigidbody rb;

    HashSet<Projectile> hitProjectiles = new HashSet<Projectile>();

    private void FixedUpdate()
    {
        if (Despawned)
            return;

        rb.MovePosition(rb.position + GameSettings.instance.enemyParameters.moveSpeed * Time.deltaTime * Vector3.back);
        rb.velocity = Vector3.zero;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Despawned)
            return;

        if (((1 << collision.gameObject.layer) & projectileLayerMask) == 0)
            return;

        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        // Only take damage from deflected projectiles
        if (!projectile.deflected)
            return;

        // Don't take damage from the same projectile more than once!
        if (hitProjectiles.Contains(projectile))
            return;

        hitProjectiles.Add(projectile);

        {   // Kill one unit
            int lastUnitIndex = units.Count - 1;
            KillUnit(units[lastUnitIndex], true);
            units.RemoveAt(lastUnitIndex);
        }

        if (Health <= 0)
            Despawn();
    }

    public void Spawn()
    {
        myCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        Despawned = false;
    }

    public void Despawn()
    {
        Spawner.instance.DespawnEnemy(gameObject);
        myCollider.enabled = false;
        Despawned = true;

        if (Health > 0)
            StartCoroutine(AttackCoroutine());
        else
            StartCoroutine(DeathCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        for (int i = 0; i < units.Count; i++)
            units[i].Animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);
        TownCollider.instance.AddOrReduceHealth(-Damage);
        Destroy(gameObject);
    }

    IEnumerator DeathCoroutine()
    {
        for (int i = 0; i < units.Count; i++)
            KillUnit(units[i], false);

        Spawner.instance.SpawnExtraProjectiles(transform.position);
        Spawner.instance.SpawnPickUp(transform.position);

        yield return new WaitForSeconds(1.8f);
        Destroy(gameObject);
    }

    void KillUnit(EnemyUnit unit, bool destroy)
    {
        unit.Animator.SetTrigger("Death");
        damageParticleEffect.Play();
        AudioController.PlayAudioClipOneShot(AudioController.instance.knifeStabClip);

        if (destroy)
            unit.SetToDestroy(2.3f);
    }

    private float Damage => Health * GameSettings.instance.enemyParameters.damage;
    private int Health => units.Count;

    public bool Despawned { get; private set; }
}
