using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private TownCollider town;

    [SerializeField]
    private LayerMask eraseLayerMask;

    public void ActivateAnnihilate()
    {
        StartCoroutine(ExpandAnnihilate());
    }

    public void HealTown()
    {
        town.AddHealth(GameSettings.instance.powerUpHealAmount);
    }

    IEnumerator ExpandAnnihilate()
    {
        float radius = 0f;
        Spawner.instance.StopSpawning();

        while (radius < GameSettings.instance.powerUpMaxRadius)
        {
            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, radius, eraseLayerMask);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    IDamageDealer dealer = colliders[0].GetComponent<IDamageDealer>();
                    dealer.Despawn();
                }
            }

            radius += GameSettings.instance.powerUpExpandSpeed * Time.deltaTime;
            yield return null;
        }

        Spawner.instance.ResetSpawning();
        Debug.Log("Power Up Ended!");
    }
}
