using TMPro;
using Unity.VisualScripting;
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

    private TextMeshProUGUI annihilateButtonText;
    private TextMeshProUGUI healButtonText;

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

        annihilateButtonText = annihilateButton.GetComponentInChildren<TextMeshProUGUI>();
        healButtonText = healButton.GetComponentInChildren<TextMeshProUGUI>();

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
