using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    public GameObject gameOverScreen;
    public Slider townHealthUISlider;
    public TextMeshProUGUI coinCountText;

    [Header("Power Up Buttons")]
    public Button annihilateButton;
    public Button healButton;

    [SerializeField] private TextMeshProUGUI annihilateButtonText;
    [SerializeField] private TextMeshProUGUI healButtonText;

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
        // Make sure this is off :P
        gameOverScreen.SetActive(false);
        UpdateCostOnButtons();
    }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        UpdateCostOnButtons();
    }
#endif

    private void UpdateCostOnButtons()
    {
        annihilateButtonText.text = $"Annihilate\n{GameSettings.instance.annihilateCost} coins";
        healButtonText.text = $"Heal Town\n{GameSettings.instance.healCost} coins";
    }
}
