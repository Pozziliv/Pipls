using UnityEngine;
using Mirror;

public class Bullet : ShootAmmo
{
    [ServerCallback]
    private void Update()
    {
        if (_rigidbody.velocity.sqrMagnitude <= 0.25f)
        {
            StopFly();
        }
    }

    [Server]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isPlayer = collision.gameObject.TryGetComponent(out PlayerManager playerManager);

        if (isPlayer)
        {
            Vector3 diff = (collision.transform.position - transform.position).normalized;

            playerManager.GetDamage(_damage, diff * _knockbackForce);
        }

        StopFly();
    }

    public override void StartFly(float rotation)
    {
        _rigidbody.AddForce(Quaternion.Euler(0, 0, rotation - 90f) * Vector2.up * 18f, ForceMode2D.Impulse);
    }

    public override void StopFly()
    {
        NetworkServer.Destroy(this.gameObject);
    }
}
