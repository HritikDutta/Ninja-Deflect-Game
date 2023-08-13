using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }

#if UNITY_EDITOR
    [SerializeField] private bool usePCControls = true;
#endif

    [Header("Joystick Settings")]
    [SerializeField] private float joystickMaxRadius = 10f;
    [SerializeField] private float joystickDeadZone = 0.025f;
    [SerializeField] private AnimationCurve joystickActivationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Joystick Visuals")]
    [SerializeField] private GameObject joystickParent;
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHead;

    private Vector2 joystickStartPosition;
    private bool inputEnabled = true;
    private bool joystickCrossedDeadZone = false;

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
        {   // Joystick Input
            HandleJoystickInput();
        }

        if (!inputEnabled)
            return;

        joystickBackground.position = joystickStartPosition;
        joystickHead.position = joystickStartPosition + AdjustedJoystickRadius * JoystickInputRaw;
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

        if (!inputEnabled)
            return;

#if UNITY_EDITOR
        if (usePCControls)
        {
            if (Input.GetMouseButtonDown(0))
            {
                joystickStartPosition = mousePosition;
                joystickParent.SetActive(true);
                joystickCrossedDeadZone = false;
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 motion = mousePosition - joystickStartPosition;
                float distance = motion.magnitude;

                float t = Mathf.InverseLerp(0f, AdjustedJoystickRadius, distance);
                joystickCrossedDeadZone = joystickCrossedDeadZone || t >= joystickDeadZone;

                if (joystickCrossedDeadZone)
                {
                    JoystickInputRaw = t * (motion / distance);
                    JoystickInput = joystickActivationCurve.Evaluate(t) * (motion / distance);
                }
            }

            if (Input.GetMouseButtonUp(0))
                joystickParent.SetActive(false);
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
                        joystickStartPosition = mousePosition;
                        joystickParent.SetActive(true);
                        joystickCrossedDeadZone = false;
                        break;
                    }

                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    {
                        Vector2 motion = mousePosition - joystickStartPosition;
                        float distance = motion.magnitude;

                        float t = Mathf.InverseLerp(0f, AdjustedJoystickRadius, distance);
                        joystickCrossedDeadZone = joystickCrossedDeadZone || t >= joystickDeadZone;

                        if (joystickCrossedDeadZone)
                        {
                            JoystickInputRaw = t * (motion / distance);
                            JoystickInput = joystickActivationCurve.Evaluate(t) * (motion / distance);
                        }
                        break;
                    }

                default:
                    {
                        joystickParent.SetActive(false);
                        break;
                    }
            }
        }
    }

    private void UpdateJoystickBackgroundSize()
    {
        joystickBackground.sizeDelta = new Vector2(2 * AdjustedJoystickRadius, 2 * AdjustedJoystickRadius);
    }

    public static Vector2 JoystickInput { get; private set; }
    public static Vector2 JoystickInputRaw { get; private set; }

    public static Transform CameraTransform { get; private set; }

    public static void SetEnabled(bool value)
    {
        instance.joystickParent.SetActive(value);
        instance.inputEnabled = value;
    }

    private float AdjustedJoystickRadius { get { return joystickMaxRadius * Screen.height / 2160f; } }
}
