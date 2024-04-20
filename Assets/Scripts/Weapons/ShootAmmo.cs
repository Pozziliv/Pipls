using Mirror;
using UnityEngine;

public abstract class ShootAmmo : NetworkBehaviour, ISpawnable
{
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected int _damage = 10;
    [SerializeField] protected float _knockbackForce = 2;

    public abstract void StopFly();

    public abstract void StartFly(float rotation);

    public void SetDirection(float angle)
    {
        transform.eulerAngles = Vector3.forward * (angle - 90f);
    }
}
