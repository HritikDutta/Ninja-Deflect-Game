using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public static PowerUpController instance;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private TownCollider town;

    [SerializeField]
    private LayerMask eraseLayerMask;

    public int coinCount = 0;

    private void Awake()
    {
        // Only one instance allowed >:(
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        ResetCoins();
    }

    public void ActivateAnnihilate()
    {
        StartCoroutine(ExpandAnnihilate());
        coinCount -= GameSettings.instance.annihilateCost;
        UpdateButtons();
    }

    public void HealTown()
    {
        town.AddHealth(GameSettings.instance.powerUpHealAmount);
        coinCount -= GameSettings.instance.annihilateCost;
        UpdateButtons();
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
                    dealer.Despawn(true);
                }
            }

            radius += GameSettings.instance.powerUpExpandSpeed * Time.deltaTime;
            yield return null;
        }

        Spawner.instance.ResetSpawning();
        Debug.Log("Power Up Ended!");
    }

    public void ResetCoins()
    {
        coinCount = 0;
        UIController.instance.coinCountText.text = $"Coins: {coinCount}";

        UIController.instance.annihilateButton.interactable = false;
        UIController.instance.healButton.interactable = false;
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UIController.instance.coinCountText.text = $"Coins: {coinCount}";
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        UIController.instance.annihilateButton.interactable = coinCount >= GameSettings.instance.annihilateCost;
        UIController.instance.healButton.interactable = coinCount >= GameSettings.instance.healCost;
    }
}
