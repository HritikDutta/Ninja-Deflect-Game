using UnityEngine;

public class TownCollider : MonoBehaviour
{
    [SerializeField] private LayerMask damageLayers;

    private float health = 0f;

    private void Start()
    {
        ResetTown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageLayers) == 0)
            return;

        IDamageDealer dealer = other.GetComponent<IDamageDealer>();

        health -= dealer.Damage;
        UIController.instance.townHealthUISlider.value = health / GameSettings.instance.townMaxHealth;

        dealer.Despawn();

        if (health <= 0f)
        {
            UIController.instance.gameOverScreen.SetActive(true);
            InputController.SetEnabled(false);
            Spawner.instance.StopSpawning();
        }
    }

    public void ResetTown()
    {
        health = GameSettings.instance.townMaxHealth;
        UIController.instance.townHealthUISlider.value = health / GameSettings.instance.townMaxHealth;
    }
}
