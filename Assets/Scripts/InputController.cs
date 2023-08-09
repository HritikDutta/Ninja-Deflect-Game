using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }

#if UNITY_EDITOR
    [SerializeField] private bool usePCControls = true;
#endif

    [Header("Joystick Settings")]
    [SerializeField] private float joystickMaxRadius = 10f;
    [SerializeField] private AnimationCurve joystickActivationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Joystick Visuals")]
    [SerializeField] private GameObject joystickParent;
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHead;

    private Vector2 joystickStartPosition;
    private bool inputEnabled = true;
    private bool joystickInputActive = false;

    private void Awake()
    {
        // Only one instance allowed >:(
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        joystickParent.SetActive(false);
        UpdateJoystickBackgroundSize();

        CameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!inputEnabled)
            return;

        {   // Joystick Input
            HandleJoystickInput();
        }

        joystickBackground.position = joystickStartPosition;
        joystickHead.position = joystickStartPosition + joystickMaxRadius * JoystickInputRaw;
    }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        UpdateJoystickBackgroundSize();
    }
#endif

    void HandleJoystickInput()
    {
        JoystickInputRaw = JoystickInput = Vector2.zero;
        Vector2 mousePosition = Input.mousePosition;

#if UNITY_EDITOR
        if (usePCControls)
        {
            if (Input.GetMouseButtonDown(0) && IsPositionInJoystickArea(mousePosition))
            {
                joystickStartPosition = mousePosition;
                joystickParent.SetActive(true);
                joystickInputActive = true;
            }

            if (joystickInputActive && Input.GetMouseButton(0))
            {
                Vector2 motion = mousePosition - joystickStartPosition;

                float t = Mathf.InverseLerp(0f, joystickMaxRadius, motion.magnitude);

                JoystickInputRaw = t * motion.normalized;
                JoystickInput = joystickActivationCurve.Evaluate(t) * motion.normalized;
            }

            if (Input.GetMouseButtonUp(0))
            {
                joystickParent.SetActive(false);
                joystickInputActive = false;
            }
        }
        else
#endif
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (IsPositionInJoystickArea(mousePosition))
                        {
                            joystickStartPosition = mousePosition;
                            joystickParent.SetActive(true);
                            joystickInputActive = true;
                        }
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (joystickInputActive)
                        {
                            Vector2 motion = mousePosition - joystickStartPosition;

                            float t = Mathf.InverseLerp(0f, joystickMaxRadius, motion.magnitude);

                            JoystickInputRaw = t * motion.normalized;
                            JoystickInput = joystickActivationCurve.Evaluate(t) * motion.normalized;
                        }
                        break;
                    }

                case TouchPhase.Ended:
                    {
                        joystickParent.SetActive(false);
                        joystickInputActive = false;
                        break;
                    }
            }
        }
    }

    private void UpdateJoystickBackgroundSize()
    {
        joystickBackground.sizeDelta = new Vector2(2 * joystickMaxRadius, 2 * joystickMaxRadius);
    }

    private bool IsPositionInJoystickArea(Vector2 position)
    {
        // Don't ovelap with the buttons at the bottom of the screen
        return position.y > 240f;
    }

    public static Vector2 JoystickInput { get; private set; }
    public static Vector2 JoystickInputRaw { get; private set; }

    public static Transform CameraTransform { get; private set; }

    public static void SetEnabled(bool value)
    {
        instance.joystickParent.SetActive(value);
        instance.inputEnabled = value;
    }
}
