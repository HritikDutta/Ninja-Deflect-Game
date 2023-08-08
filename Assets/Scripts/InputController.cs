using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set; }

    [SerializeField] private float joystickMaxRadius = 10f;

    [SerializeField] private AnimationCurve joystickActivationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Joystick Visuals")]
    [SerializeField] private GameObject joystickParent;
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHead;

    private Vector2 joystickStartPosition;
    private bool inputEnabled = true;

    private void Start()
    {
        // Only one instance allowed >:(
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        joystickParent.SetActive(false);
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

    void HandleJoystickInput()
    {
        JoystickInputRaw = JoystickInput = Vector2.zero;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            joystickStartPosition = Input.mousePosition;
            joystickParent.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 motion = mousePosition - joystickStartPosition;

            float t = Mathf.InverseLerp(0f, joystickMaxRadius, motion.magnitude);

            JoystickInputRaw = t * motion.normalized;
            JoystickInput = joystickActivationCurve.Evaluate(t) * motion.normalized;
        }

        if (Input.GetMouseButtonUp(0))
            joystickParent.SetActive(false);
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        joystickStartPosition = Input.mousePosition;
                        joystickParent.SetActive(true);
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        Vector2 mousePosition = Input.mousePosition;
                        Vector2 motion = mousePosition - joystickStartPosition;

                        float t = Mathf.InverseLerp(0f, joystickMaxRadius, motion.magnitude);

                        JoystickInputRaw = t * motion.normalized;
                        JoystickInput = joystickActivationCurve.Evaluate(t) * motion.normalized;
                        break;
                    }

                case TouchPhase.Ended:
                    {
                        joystickParent.SetActive(false);
                        break;
                    }
            }
        }
#endif
    }

    public static Vector2 JoystickInput { get; private set; }
    public static Vector2 JoystickInputRaw { get; private set; }

    public static void SetEnabled(bool value)
    {
        instance.joystickParent.SetActive(value);
        instance.inputEnabled = value;
    }
}
