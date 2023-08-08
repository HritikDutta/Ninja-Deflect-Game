using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    public GameObject gameOverScreen;

    private void Start()
    {
        // Only one instance allowed >:(
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;

        // Make sure this is off :P
        gameOverScreen.SetActive(false);
    }
}
