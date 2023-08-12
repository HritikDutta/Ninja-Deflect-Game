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

    private int coinCount = 0;

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
        Coins -= GameSettings.instance.annihilateCost;
        UpdateButtons();
    }

    public void HealTown()
    {
        town.AddOrReduceHealth(GameSettings.instance.powerUpHealAmount);
        Coins -= GameSettings.instance.healCost;
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
                    ISpawnObject spawnObject = colliders[0].GetComponent<ISpawnObject>();
                    spawnObject.Despawn();
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
        Coins = 0;

        UIController.instance.annihilateButton.interactable = false;
        UIController.instance.healButton.interactable = false;
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateButtons();
    }

    private int Coins
    {
        get { return coinCount; }
        set {
            coinCount = value;
            UIController.instance.coinCountText.text = $"{coinCount}";
        }
    }

    private void UpdateButtons()
    {
        UIController.instance.annihilateButton.interactable = coinCount >= GameSettings.instance.annihilateCost;
        UIController.instance.healButton.interactable = coinCount >= GameSettings.instance.healCost;
    }
}
