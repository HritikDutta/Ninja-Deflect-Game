using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

public class TownCollider : MonoBehaviour
{
    [SerializeField] private LayerMask damageLayers;
    
    private MMF_Player feelPackagePlayer;
    private HapticSource hapticSource;

    private float health = 0f;

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

        ISpawnObject dealer = other.GetComponent<ISpawnObject>();
        dealer.Despawn(this);

        if (health <= 0f)
        {
            UIController.instance.gameOverScreen.SetActive(true);
            InputController.SetEnabled(false);
            Spawner.instance.StopSpawning();
        }
    }

    public void AddOrReduceHealth(float additional)
    {
        health = Mathf.Min(health + additional, GameSettings.instance.townMaxHealth);
        UIController.instance.townHealthUISlider.value = health / GameSettings.instance.townMaxHealth;

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
        health = GameSettings.instance.townMaxHealth;
        UIController.instance.townHealthUISlider.value = health / GameSettings.instance.townMaxHealth;
    }
}
