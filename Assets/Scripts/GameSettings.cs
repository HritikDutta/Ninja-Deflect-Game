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

    public BehaviourParameters projectileParameters;
    public BehaviourParameters enemyParameters;

    public float projectileDeflectSpeed = 1.2f;

    public int enemyMaxHealth = 5;
    public float townMaxHealth = 100;
}
