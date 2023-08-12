using UnityEngine;

public class PickUp : MonoBehaviour, ISpawnObject
{
    [SerializeField]
    private Transform visualTransform;

    private bool collected = false;

    public void Spawn()
    {
        collected = false;
    }

    public void Despawn(TownCollider town)
    {
    }

    private void Update()
    {
        transform.Rotate(0f, 600f * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        // No need to check layer since it only interacts with the player
        visualTransform.gameObject.SetActive(false);

        collected = true;
        Destroy(gameObject);
    }
}
