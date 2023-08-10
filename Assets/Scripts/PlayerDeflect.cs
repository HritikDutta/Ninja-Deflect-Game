using UnityEngine;

public class PlayerDeflect : MonoBehaviour
{
    [SerializeField]
    private LayerMask projectileLayerMask;

    [SerializeField]
    private ParticleSystem deflectEffect;

    private Animator animator;

    private void Start()
    {
        animator = transform.parent.GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & projectileLayerMask) == 0)
            return;
        
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile.deflected)
            return;

        animator.SetTrigger("Deflect");
        deflectEffect.Play();

        if (Spawner.instance.spawnedEnemies.Count > 0)
        {
            Enemy enemy = Spawner.instance.spawnedEnemies[0].GetComponent<Enemy>();
            projectile.Deflect((enemy.transform.position - other.transform.position).normalized);
            enemy.AddDeflectedProjectile(projectile);
        }
        else
        {
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(0f, 1f));
            projectile.Deflect(randomDirection.normalized);
        }
    }
}
