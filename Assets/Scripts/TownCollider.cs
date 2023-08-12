using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

public class TownCollider : MonoBehaviour
{
    public static TownCollider instance;

    [SerializeField] private LayerMask damageLayers;
    
    private MMF_Player feelPackagePlayer;
    private HapticSource hapticSource;

    private void Awake()
    {
        // Only one instance allowed >:(
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        feelPackagePlayer = GetComponent<MMF_Player>();
        hapticSource = GetComponent<HapticSource>();

        ResetTown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageLayers) == 0)
            return;

        ISpawnObject spawnObject = other.GetComponent<ISpawnObject>();
        spawnObject.Despawn();

        if (Health <= 0f)
        {
            UIController.instance.gameOverScreen.SetActive(true);
            InputController.SetEnabled(false);
            Spawner.instance.StopSpawning();
        }
    }

    public void AddOrReduceHealth(float additional)
    {
        Health = Mathf.Min(Health + additional, GameSettings.instance.townMaxHealth);
        UIController.instance.townHealthUISlider.value = Health / GameSettings.instance.townMaxHealth;

        if (additional < 0f)
        {
            feelPackagePlayer.PlayFeedbacks();

            // More damage == More vibration
            hapticSource.level = -additional / (GameSettings.instance.enemyParameters.damage * 4);
            hapticSource.Play();
        }
    }

    public void ResetTown()
    {
        Health = GameSettings.instance.townMaxHealth;
        UIController.instance.townHealthUISlider.value = Health / GameSettings.instance.townMaxHealth;
    }

    public float Health { get; private set; }
}
