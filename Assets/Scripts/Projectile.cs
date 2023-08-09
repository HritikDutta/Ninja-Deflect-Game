using UnityEngine;

public class Projectile : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float despawnRadius = 50f;

    [HideInInspector] public Vector3 moveDirection;
    public bool deflected = false;

    private Transform mainCameraTransform;

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    public void StartMoving()
    {
        moveDirection = Vector3.back;
        deflected = false;
    }

    public void Deflect(Vector3 direction)
    {
        moveDirection = direction;
        deflected = true;
    }

    private void LateUpdate()
    {
        float speed = deflected ? GameSettings.instance.projectileDeflectSpeed : GameSettings.instance.projectileParameters.moveSpeed;
        transform.position += speed * Time.deltaTime * moveDirection;

        if (TooFarFromCamera())
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Spawner.instance.DespawnProjectile(gameObject);
    }

    private bool TooFarFromCamera()
    {
        Vector3 difference = transform.position - mainCameraTransform.position;
        difference.y = 0f;
        return difference.sqrMagnitude > despawnRadius * despawnRadius;
    }

    public float Damage => GameSettings.instance.projectileParameters.damage;
}