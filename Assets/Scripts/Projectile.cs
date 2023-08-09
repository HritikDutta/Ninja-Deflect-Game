using UnityEngine;

public class Projectile : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float despawnRadius = 50f;

    [HideInInspector] public Vector3 moveDirection;
    public bool deflected = false;

    public void Deflect(Vector3 direction)
    {
        moveDirection = direction;
        deflected = true;
    }

    private void LateUpdate()
    {
        float speed = deflected ? GameSettings.instance.projectileDeflectSpeed : GameSettings.instance.projectileParameters.moveSpeed;
        transform.position += speed * Time.deltaTime * moveDirection;

        transform.Rotate(Vector3.up * 3f * speed);

        if (TooFarFromCamera())
            Despawn(false);
    }

    public void Spawn()
    {
        moveDirection = Vector3.back;
        deflected = false;
    }

    public void Despawn(bool isDeath)
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

    public float Damage => deflected ? GameSettings.instance.projectileParameters.damage : 0f;
}
