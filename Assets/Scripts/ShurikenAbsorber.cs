using UnityEngine;

public class ShurikenAbsorber : MonoBehaviour
{
    [SerializeField] private LayerMask projectileLayerMask;

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemy.Despawned)
            return;

        if (((1 << other.gameObject.layer) & projectileLayerMask) == 0)
            return;

        Projectile projectile = other.GetComponent<Projectile>();

        // Absorb projectiles if they haven't been deflected yet
        if (!projectile.deflected)
            projectile.Despawn(null);
    }
}
