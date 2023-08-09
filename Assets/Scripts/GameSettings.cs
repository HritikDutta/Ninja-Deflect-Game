using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    [System.Serializable]
    public struct BehaviourParameters
    {
        public float damage;
        public float moveSpeed;
    }

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

    [Header("Projectile Settings")]
    public BehaviourParameters projectileParameters;
    public float projectileDeflectSpeed = 1.2f;

    [Header("Enemy Settings")]
    public BehaviourParameters enemyParameters;
    public int enemyMaxHealth = 5;

    [Header("Town Settings")]
    public float townMaxHealth = 100;
}
