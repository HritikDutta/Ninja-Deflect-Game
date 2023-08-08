using UnityEngine;

public class TownCollider : MonoBehaviour
{
    [SerializeField] private LayerMask damageLayers;
    [SerializeField] private Spawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageLayers) == 0)
            return;

        spawner.DespawnObject(other.gameObject);
    }
}
