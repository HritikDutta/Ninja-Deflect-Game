using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startPosition;

    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("MoveX", InputController.JoystickInput.x);
        animator.SetFloat("MoveZ", InputController.JoystickInput.y);
    }

    void FixedUpdate()
    {
        Vector2 movement = GameSettings.instance.playerMoveSpeed * Time.fixedDeltaTime * InputController.JoystickInput;

        Vector3 targetPosition = rb.position;
        targetPosition.x += movement.x;
        targetPosition.z += movement.y;

        rb.MovePosition(targetPosition);
        rb.velocity = Vector3.zero;
    }

    public void ResetPlayer()
    {
        rb.position = startPosition;
    }
}
