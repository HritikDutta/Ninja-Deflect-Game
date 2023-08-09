using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    public GameObject gameOverScreen;
    public Slider townHealthUISlider;

    public Button[] powerUps;

    private void Awake()
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
