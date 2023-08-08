using UnityEngine;

public class PlayerDeflect : MonoBehaviour
{
    [SerializeField] private LayerMask projectileLayerMask;
    [SerializeField] private Spawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & projectileLayerMask) == 0)
            return;

        spawner.DeflectProjectile(other.gameObject);
    }
}
