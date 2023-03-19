using UnityEngine;

public class Character : MonoBehaviour
{
    private Vector3 _velocity = Vector3.zero;

    private Vector3 _acceleration = Vector3.zero;

    private const float AccelerationMagnitude = 2;

    private const float MaxVelocityMagnitude = 5;

    private const float DamagePerSecond = 50;

    private const float DamageRange = 10;

    private void Update()
    {
        Move();
        DamageNearbyShapes();
        UpdateAcceleration();
    }

    private void Move()
    {
        _velocity += _acceleration * Time.deltaTime;
        if (_velocity.magnitude > MaxVelocityMagnitude)
        {
            _velocity = _velocity.normalized * MaxVelocityMagnitude;
        }
        transform.position += _velocity * Time.deltaTime;
    }

    private void UpdateAcceleration()
    {
        var direction = Vector3.zero;
        var currentPosition = transform.position;
        var nearbyColliders = new Collider2D[16];
        var colliderCount = Physics2D.OverlapCircleNonAlloc(currentPosition, DamageRange, nearbyColliders);

        for (int i = 0; i < colliderCount; i++)
        {
            if (nearbyColliders[i].TryGetComponent<Circle>(out var circle))
            {
                direction += (circle.transform.position - currentPosition) * circle.Health;
            }
        }
        _acceleration = direction.normalized * AccelerationMagnitude;
    }

    private void DamageNearbyShapes()
    {
        Vector3 pos = transform.position;
        Collider2D[] nearbyColliders = new Collider2D[10];
        int count = Physics2D.OverlapCircleNonAlloc(pos, DamageRange, nearbyColliders);
        
        if (count == 0)
        {
            transform.position = Vector3.zero;
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (nearbyColliders[i].TryGetComponent(out Circle circle))
            {
                circle.ReceiveHp(-DamagePerSecond * Time.deltaTime);
            }
        }
    }
}
