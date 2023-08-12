using UnityEngine;

public class Projectile : MonoBehaviour, ISpawnObject
{
    [SerializeField] private float despawnRadius = 50f;
    [SerializeField] private Transform visualTransform;

    [HideInInspector] public Vector3 moveDirection;
    public bool deflected = false;

    private Rigidbody rb;

    public void Deflect(Vector3 direction)
    {
        moveDirection = direction;
        deflected = true;
    }

    private void LateUpdate()
    {
        visualTransform.Rotate(0f, 600f * Speed * Time.deltaTime, 0f);

        if (TooFarFromCamera())
            DespawnInternal();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Speed * Time.fixedDeltaTime * moveDirection);
        rb.velocity = Vector3.zero;
    }

    public void Spawn()
    {
        rb = GetComponent<Rigidbody>();

        moveDirection = Vector3.back;
        deflected = false;
    }

    public void Despawn()
    {
        TownCollider.instance.AddOrReduceHealth(-Damage);
        DespawnInternal();
    }

    private void DespawnInternal()
    {
        Spawner.instance.DespawnProjectile(gameObject);
        Destroy(gameObject);
    }

    private bool TooFarFromCamera()
    {
        Vector3 difference = transform.position - InputController.CameraTransform.position;
        difference.y = 0f;
        return difference.sqrMagnitude > despawnRadius * despawnRadius;
    }

    private float Damage => deflected ? GameSettings.instance.projectileParameters.damage : 0f;
    private float Speed => deflected ? GameSettings.instance.projectileDeflectSpeed : GameSettings.instance.projectileParameters.moveSpeed;
}
